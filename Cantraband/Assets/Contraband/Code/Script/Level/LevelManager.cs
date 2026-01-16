using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

public class LevelManager : MonoBehaviour
{
    private const string SOUND_NEW_CLIENT = "FOL_buyers_arrive";
    private const string SOUND_EXIT_CLIENT = "FOL_departure";
    private const string SOUND_GIVE_FAVORITEGAME = "SFX_Super_Deal";
    private const string SOUND_GIVE_GOODCATEGORY = "SFX_Validation_Deal";
    private const string SOUND_GIVE_WRONGGAME = "SFX_deal_Wrong_Game";
    private const string SOUND_GIVE_WRONGGAMETOCOP = "SFX_deal_Failed";

    private const int SOUND_WAIT_DELAY = 500;

    public static LevelManager Instance { get; private set; }

    [Header("Transaction Cooldown")]
    [SerializeField] private float _transactionCheckCoolDown;

    [Header("Score")] public int score;
    [SerializeField] private int _pointGoodCategory = 1;
    [SerializeField] private int _pointGoodGame = 3;
    [SerializeField] private int _pointWrongGame = -1;

    [Header("Clients")]
    [SerializeField] private Transform _clientSpawnPoint;

    [Header("Timer")]
    [SerializeField] private bool _activateGameTimer = true;
    [SerializeField] private float _gameTimer;

    [Header("Events")]
    public UnityEvent onClientLeaveEvent;
    [SerializeField] private UnityEvent _onGameOver;
    [SerializeField] private UnityEvent _onGameEnded;

    //Clients fields
    private int _numberClient = 0; //Nombre de client rencontrés
    private bool _hasClient = false; //est ce que le client existe
    private Client _currentClient;
    private GameObject _currentClientObject;
    //Check same client fields
    private int _lastClient;
    private int _currentNumberOfEncounteredSameClients;

    //Timer fields
    private Coroutine _timerRoutine;

    //Transactions fields
    private Coroutine _btwTransactionRoutine;
    private Coroutine _isTransitionCoolDownRoutine;
    private bool _isRightAfterTransaction = false;

    //Stats fields
    private List<GameCard> _gameCardsGiven = new List<GameCard>();
    private List<int> _pointsAwarded = new List<int>();

    //Getter / setter
    public int NumberOfClientsEncountered { get => _numberClient; }
    public bool IsBetweenTransactions { get; private set; }
    public bool IsGameRunning { get; private set; } //AKA pas en game over
    public bool IsGameTimerActive { get => _activateGameTimer; }
    public List<GameCard> GameCardsGiven { get => _gameCardsGiven; }
    public List<int> PointsAwarded { get => _pointsAwarded; }
    public float RemainingTime { get; private set; }
    public float GameTime { get => _gameTimer; }

    //Actions
    public Action<GameReturnedType> OnGameReturned;
    public Action OnReceiveCartridge;


    public Action OnFinishTransaction;
    public Action<Client> onNextClientAction;
    public Action OnFailedByCop; //Quand donne un "mauvais" jeu au flic infiltré
    public Action OnGameOver;
    public Action OutOfPatience; //Quand le client n'a plus de patience
    public Action OnSpottedByCop;
    public Action OnTimerFinished;

    public Action OnDebugStopTimer;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        IsGameRunning = true;
    }

    private void OnEnable()
    {
        InputManager.Instance.OnReadCard += StartPlayerGiveCard;
    }

    private void OnDisable()
    {
        InputManager.Instance.OnReadCard -= StartPlayerGiveCard;

        if (_btwTransactionRoutine != null)
        {
            StopCoroutine(_btwTransactionRoutine);
            _btwTransactionRoutine = null;
        }
    }

    private void Start()
    {
        if(_activateGameTimer)
        {
            RemainingTime = _gameTimer;
            _timerRoutine = StartCoroutine(StartTimer());
        }

        _btwTransactionRoutine = StartCoroutine(GetFirstClient());
    }

    private void Update()
    {
        if (Keyboard.current[Key.F5].wasPressedThisFrame)
        {
            _activateGameTimer = false;
            OnDebugStopTimer?.Invoke();
            if(_timerRoutine != null)
            {
                StopCoroutine(_timerRoutine);
                _timerRoutine = null;
            }
        }
    }

    #region Clients functions
    private IEnumerator GetFirstClient()
    {
        yield return new WaitForSeconds(RythmManager.Instance.RandomTimeBeforeNextClient);
        GiveNextClient();
        AudioManager.AudioManager.Instance.PlaySound(SOUND_NEW_CLIENT);
    }

    public void GiveNextClient()
    {
        //Check if game is running
        if (!IsGameRunning)
            return;

        //Get next client
        _currentClient = GetRandomClient();
        _hasClient = true;
        _numberClient++;

        //Spawn next client
        _currentClientObject = Instantiate(_currentClient.clientPrefab, _clientSpawnPoint.position, Quaternion.identity);
        _currentClientObject.GetComponent<ClientBehaviour>().SetupClient(_currentClient.favoriteCard.hintImage);
        AudioManager.AudioManager.Instance.PlaySound(SOUND_NEW_CLIENT);

        onNextClientAction?.Invoke(_currentClient);
    }

    private Client GetRandomClient()
    {
        if (DataContainer.Clients.Count == 0)
        {
            throw new Exception("Missing Client");
        }

        int randomIndex = 0;
        do
        {
            randomIndex = UnityEngine.Random.Range(0, DataContainer.Clients.Count);
        } while (RythmManager.Instance.MaxNumberOfSameClient <= _currentNumberOfEncounteredSameClients && _lastClient == randomIndex); //Prevent same client to appear

        if (_lastClient != randomIndex)
            _currentNumberOfEncounteredSameClients = 1;
        else
            _currentNumberOfEncounteredSameClients++;
        _lastClient = randomIndex;

        return DataContainer.Clients[randomIndex];
    }

    public void StartClientNoMorePatience()
    {
        //Check if not in game over
        if (!IsGameRunning) return;

        //Check if there's a client
        if (!_hasClient) return;

        StartCoroutine(ClientNoMorePatience());
    }
    private IEnumerator ClientNoMorePatience() //Client leaves when no more patience
    {
        _hasClient = false;
        OutOfPatience?.Invoke();

        //Complete transaction
        AudioManager.AudioManager.Instance.PlaySound(SOUND_EXIT_CLIENT);
        yield return new WaitForSeconds(RythmManager.Instance.RandomTimeBeforeNextClient);
        GiveNextClient();
    }
    #endregion

    #region Transactions functions
    public void StartPlayerGiveCard(string tag) => _btwTransactionRoutine = StartCoroutine(PlayerGiveCard(tag));
    public IEnumerator PlayerGiveCard(string tag)
    {
        //Check if not in game over
        if (!IsGameRunning) yield break;

        //Check if there's a client
        if (!_hasClient) yield break;
        _hasClient = false;

        //Get card
        GameCard selectedCard = GetCardGame(tag);
        if (selectedCard == null)
            throw new Exception($"No card with tag: {tag}");

        OnReceiveCartridge?.Invoke();
        //Check police
        if (_currentClient.isPolice)
        {
            if (!selectedCard.genres.Contains(GameGenre.Factice))
            {
                OnFailedByCop?.Invoke(); //Passe par le flic pour jouer l'anim avant de lancer le game over
                AudioManager.AudioManager.Instance.PlaySound(SOUND_GIVE_WRONGGAMETOCOP);
                yield break;
            }

            //Complete (fake) transaction
            OnFinishTransaction?.Invoke();
            OnGameReturned?.Invoke(GameReturnedType.Good);
            AudioManager.AudioManager.Instance.PlaySound(SOUND_GIVE_GOODCATEGORY, SOUND_WAIT_DELAY);
            AudioManager.AudioManager.Instance.PlaySound(SOUND_EXIT_CLIENT);
            yield return new WaitForSeconds(RythmManager.Instance.RandomTimeBeforeNextClient);
            GiveNextClient();
            yield break;
        }

        //compute score
        GameReturnedType clientResponse = ComputeScore(selectedCard);
        //Launch popup feedback
        OnGameReturned?.Invoke(clientResponse);

        //Complete transaction
        OnFinishTransaction?.Invoke();
        AudioManager.AudioManager.Instance.PlaySound(SOUND_EXIT_CLIENT);

        //Activate transaction cooldown
        if (_isTransitionCoolDownRoutine != null)
        {
            StopCoroutine(_isTransitionCoolDownRoutine);
            _isTransitionCoolDownRoutine = null;
        }
        _isTransitionCoolDownRoutine = StartCoroutine(TransitionCoolDown());

        yield return new WaitForSeconds(RythmManager.Instance.RandomTimeBeforeNextClient);
        GiveNextClient();
    }

    private GameCard GetCardGame(string tag)
    {
        foreach (GameCard card in DataContainer.GameCards)
        {
            if (card.tag == tag || card.DebugKeyboardTag == tag)
                return card;
        }
        return null;
    }

    private IEnumerator TransitionCoolDown()
    {
        _isRightAfterTransaction = true;
        yield return new WaitForSeconds(_transactionCheckCoolDown);
        _isRightAfterTransaction = false;
    }

    public enum GameReturnedType { Wrong, Good, Favorite }
    private GameReturnedType ComputeScore(GameCard gameCard) //return what type of game was given
    {
        //Search favorite came
        if (_currentClient.favoriteCard == gameCard)
        {
            score += _pointGoodGame;
            _gameCardsGiven.Add(gameCard);
            _pointsAwarded.Add(_pointGoodGame);
            OnGameReturned?.Invoke(GameReturnedType.Favorite);
            AudioManager.AudioManager.Instance.PlaySound(SOUND_GIVE_FAVORITEGAME, SOUND_WAIT_DELAY);
            return GameReturnedType.Favorite;
        }

        //Else search good category
        foreach (GameGenre genre in gameCard.genres)
        {
            if (_currentClient.genrePreference.Contains(genre))
            {
                score += _pointGoodCategory;
                _gameCardsGiven.Add(gameCard);
                _pointsAwarded.Add(_pointGoodCategory);
                OnGameReturned?.Invoke(GameReturnedType.Good);
                AudioManager.AudioManager.Instance.PlaySound(SOUND_GIVE_GOODCATEGORY, SOUND_WAIT_DELAY);
                return GameReturnedType.Good;
            }
        }

        //Else remove points
        score += _pointWrongGame;
        score = Mathf.Max(score, 0);
        _gameCardsGiven.Add(gameCard);
        _pointsAwarded.Add(_pointWrongGame);
        OnGameReturned?.Invoke(GameReturnedType.Wrong);
        AudioManager.AudioManager.Instance.PlaySound(SOUND_GIVE_WRONGGAME, SOUND_WAIT_DELAY);
        return GameReturnedType.Wrong;
    }
    #endregion

    #region Game over
    public bool CheckPlayerCoat(bool launchGameOver = true)
    {
        if (!InputManager.Instance.IsVestOpened)
            return false;

        if(launchGameOver)
            LaunchGameOver();

        return true;
    }

    public bool CheckPlayerTransaction(bool launchGameOver = true)
    {
        if (!_isRightAfterTransaction)
            return false;

        if (launchGameOver)
            LaunchGameOver();

        return true;
    }

    [Button]
    public void LaunchGameOver()
    {
        if (!IsGameRunning) return;
        Debug.Log("Game over");

        IsGameRunning = false;
        _onGameOver?.Invoke();
        OnGameOver?.Invoke();
    }
    #endregion

    #region Timer

    private IEnumerator StartTimer()
    {
        while (RemainingTime > 0)
        {
            RemainingTime -= Time.deltaTime;
            yield return null;
        }
        OnTimerFinished?.Invoke();
        _onGameEnded?.Invoke();
    }

    #endregion

    private async void cutAllsounds()
    {
        await WaitSeconds(500);
        AudioManager.AudioManager.Instance.CutAllSounds();
    }

    private async Task WaitSeconds(int seconds)
    {
        await Task.Delay(seconds * 1000);
    }
}