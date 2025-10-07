using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class LevelManager : MonoBehaviour
{
    private const string SOUND_NEW_CLIENT = "Client_New";
    private const string SOUND_EXIT_CLIENT = "Client_Exit";
    private const string SOUND_GIVE_FAVORITEGAME = "Give_FavoriteCard";
    private const string SOUND_GIVE_GOODCATEGORY = "Give_GoodCategory";
    private const string SOUND_GIVE_WRONGGAME = "Give_WrongGame";

    public static LevelManager Instance { get; private set; }

    [Header("Transaction Cooldown")]
    [SerializeField] private float _transactionCheckCoolDown;

    [Header("Score"), NaughtyAttributes.ReadOnly] public int score;
    [SerializeField] private int _pointGoodCategory = 1;
    [SerializeField] private int _pointGoodGame = 3;
    [SerializeField] private int _pointWrongGame = -1;
    [SerializeField, MinMaxSlider(1,10)] private Vector2 _timeBeforeNextClient = new Vector2(3,5);

    [Header("Events")]
    public UnityEvent onClientLeaveEvent;
    [SerializeField] private UnityEvent _onGameOver;

    private bool _hasClient = false; //est ce que le client existe

    private Coroutine _isTransitionCoolDownRoutine;
    private bool _isRightAfterTransaction = false;

    public List<GameCard> _gameCards;
    public List<Client> _clients;
    private Client _currentClient;

    public bool IsBetweenTransactions { get; private set; }
    public bool IsGameRunning { get; private set; } //AKA pas en game over

    public Action OnValidateTransaction;
    public Action<Client> onNextClientAction;
    public Action OnGameOver;

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
        InputManager.Instance.OnReadCard += PlayerGiveCard;
    }

    private void OnDisable()
    {
        InputManager.Instance.OnReadCard -= PlayerGiveCard;
    }

    private void Start()
    {
        GetFirstClient();
    }

    private async void GetFirstClient()
    {
        await WaitSeconds((int)Random.Range(_timeBeforeNextClient.x, _timeBeforeNextClient.y));
        GiveNextClient();
        AudioManager.AudioManager.Instance.PlaySound(SOUND_NEW_CLIENT);
    }

    public async void PlayerGiveCard(int tag)
    {
        //Check if not in game over
        if (!IsGameRunning) return;

        //Check if vest is opened
        if (!InputManager.Instance.IsVestOpened)
        {
            Debug.LogWarning("Open vest first !");
            return;
        }

        //Check if there's a client
        if (!_hasClient) return;
        _hasClient = false;

        //Get card & compute score
        GameCard selectedCard = GetCardGame(tag);
        if (selectedCard == null)
            throw new Exception($"No card with tag {tag}");
        ComputeScore(selectedCard);

        //Complete transaction
        OnValidateTransaction?.Invoke();
        AudioManager.AudioManager.Instance.PlaySound(SOUND_EXIT_CLIENT);

        //Activate transaction cooldown
        if (_isTransitionCoolDownRoutine != null)
        {
            StopCoroutine(_isTransitionCoolDownRoutine);
            _isTransitionCoolDownRoutine = null;
        }
        _isTransitionCoolDownRoutine = StartCoroutine(TransitionCoolDown());

        await WaitSeconds((int)Random.Range(_timeBeforeNextClient.x, _timeBeforeNextClient.y));
        GiveNextClient();
    }

    public void GiveNextClient()
    {
        _currentClient = GetRandomClient();
        _hasClient = true;

        onNextClientAction?.Invoke(_currentClient);
    }

    private GameCard GetCardGame(int tag)
    {
        foreach(GameCard card in _gameCards)
        {
            if(card.tag == tag)
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

    private void ComputeScore(GameCard gameCard)
    {
        //Search favorite came
        if (_currentClient.favoriteCard == gameCard)
        {
            score += _pointGoodGame;
            AudioManager.AudioManager.Instance.PlaySound(SOUND_GIVE_FAVORITEGAME);
            return;
        }

        //Else search good category
        foreach (GameGenre genre in gameCard.genres)
        {
            if (_currentClient.genrePreference.Contains(genre))
            {
                score += _pointGoodCategory;
                AudioManager.AudioManager.Instance.PlaySound(SOUND_GIVE_GOODCATEGORY);
                return;
            }
        }

        //Else remove points
        score -= _pointWrongGame;
        AudioManager.AudioManager.Instance.PlaySound(SOUND_GIVE_WRONGGAME);
    }

    Client GetRandomClient()
    {
        if (_clients.Count == 0)
        {
            Debug.LogError("Missing Client");
            return new();
        }
        int randomIndex = UnityEngine.Random.Range(0, _clients.Count);
        return _clients[randomIndex];
    }

    #region Game over
    public void CheckPlayerCoat()
    {
        if (!InputManager.Instance.IsVestOpened)
            return;

        LaunchGameOver();
    }

    public void CheckPlayerTransaction()
    {
        if (!_isRightAfterTransaction)
            return;

        LaunchGameOver();
    }

    private void LaunchGameOver()
    {
        if (!IsGameRunning) return;

        Debug.Log("GAME OVER !");
        IsGameRunning = false;
        _onGameOver?.Invoke();
        OnGameOver?.Invoke();
    }
    #endregion

private async Task WaitSeconds(int seconds)
    {
        IsBetweenTransactions = true;
        await Task.Delay(seconds * 1000);
        IsBetweenTransactions = false;
    }
}