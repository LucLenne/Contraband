using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }
    private Dictionary<int, GameCard> _gameCards;
    [SerializeField]private List<Client> _clients;
    private Client _currentClient;
    [Header("Score")]private int _score;
    public int pointGoodCategory;
    public int pointGoodGame;

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

    public System.Action onNextClientAction;
    public UnityEvent onNextClientEvent;

    private GameCard GetCardGame(int tag)
    {
        return _gameCards[tag];
    }

    public void NextClient(int tag)
    {
        ComputeScore(GetCardGame(tag));
        _currentClient = GetRandomClient();
        onNextClientEvent?.Invoke();
        onNextClientAction?.Invoke(_currentClient);
    }

    private void ComputeScore(GameCard gameCard)
    {
        //Search favorite came
        if (_currentClient.favoriteCard == gameCard) 
        {
            _score += pointGoodGame;
        }

        //Else search good category
        foreach (GameGenre genre in gameCard.genres)
        {
            foreach (GameGenre genre in gameCard.genres)
            {
                if (_currentClient.genrePreference.Contains(genre))
                {
                    _score += pointGoodCategory;
                    return;
                }
            }
        }

        //Else remove points
        _score -= pointWrongGame;
    }

    private Client GetRandomClient()
    {
        if (_clients.Count == 0)
        {
            Debug.LogError("Missing Client");
            return new();
        }
        int randomIndex = Random.Range(0, _clients.Count);
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
