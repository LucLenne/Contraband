using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    [Header("Transaction Cooldown")]
    [SerializeField] private float _transactionCheckCoolDown;

    [Header("Score")] private int _score;
    [SerializeField] private int pointGoodCategory;
    [SerializeField] private int pointGoodGame;
    [SerializeField] private int pointWrongGame;

    [Header("Events")]
    public UnityEvent onNextClientEvent;
    [SerializeField] private UnityEvent _onGameOver;

    private Coroutine _isTransitionCoolDownRoutine;
    private bool _isInTransaction = false;

    private Dictionary<int, GameCard> _gameCards;
    private List<Client> _clients;
    private Client _currentClient;

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
    }

    private void OnEnable()
    {
        PopUpManager.Instance.OnCheckPlayerCoat += CheckPlayerCoat;
        InputManager.Instance.OnReadCard += NextClient;
    }

    private void OnDisable()
    {
        PopUpManager.Instance.OnCheckPlayerCoat -= CheckPlayerCoat;
        InputManager.Instance.OnReadCard -= NextClient;
    }

    public void NextClient(int tag)
    {
        ComputeScore(GetCardGame(tag));
        //Activate transaction cooldown
        if(_isTransitionCoolDownRoutine != null)
        {
            StopCoroutine(_isTransitionCoolDownRoutine);
            _isTransitionCoolDownRoutine = null;
        }
        _isTransitionCoolDownRoutine = StartCoroutine(TransitionCoolDown());

        _currentClient = GetRandomClient();
        onNextClientEvent?.Invoke();
        onNextClientAction?.Invoke(_currentClient);
    }
    private GameCard GetCardGame(int tag)
    {
        return _gameCards[tag];
    }

    private IEnumerator TransitionCoolDown()
    {
        _isInTransaction = true;
        yield return new WaitForSeconds(_transactionCheckCoolDown);
        _isInTransaction = false;
    }

    private void ComputeScore(GameCard gameCard)
    {
        //Search favorite came
        if (_currentClient.favoriteCard == gameCard)
        {
            _score += pointGoodGame;
            return;
        }

        //Else search good category
        foreach (GameGenre genre in gameCard.genres)
        {
            if (_currentClient.genrePreference.Contains(genre))
            {
                _score += pointGoodCategory;
                return;
            }
        }

        //Else remove points
        _score -= pointWrongGame;
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
    private void CheckPlayerCoat()
    {
        if (!InputManager.Instance.IsVestOpened)
            return;

        LaunchGameOver();
    }

    private void CheckPlayerTransaction()
    {
        if (!_isInTransaction)
            return;

        LaunchGameOver();
    }

    private void LaunchGameOver()
    {
        Debug.Log("GAME OVER !");
        _onGameOver?.Invoke();
        OnGameOver?.Invoke();
    }
    #endregion
}