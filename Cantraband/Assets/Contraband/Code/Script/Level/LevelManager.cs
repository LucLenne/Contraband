using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    [Header("Score")] private int _score;
    [SerializeField] private int pointGoodCategory;
    [SerializeField] private int pointGoodGame;
    [SerializeField] private int pointWrongGame;

    [Header("Events")]
    public UnityEvent onNextClientEvent;
    [SerializeField] private UnityEvent _onGameOver;

    private Dictionary<int, GameCard> _gameCards;
    private List<Client> _clients;
    private Client _currentClient;

    public Action onNextClientAction;
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
    }

    private void OnDisable()
    {
        PopUpManager.Instance.OnCheckPlayerCoat -= CheckPlayerCoat;
    }

    GameCard GetCardGame(int tag)
    {
        return _gameCards[tag];
    }

    public void NextClient(int tag)
    {
        ComputeScore(GetCardGame(tag));
        _currentClient = GetRandomClient();
        onNextClientEvent?.Invoke();
        onNextClientAction?.Invoke();
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

    private void CheckPlayerCoat()
    {
        if (!InputManager.Instance.IsVestOpened)
            return;

        Debug.Log("GAME OVER !");
        _onGameOver?.Invoke();
        OnGameOver?.Invoke();
    }
}
