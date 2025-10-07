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
    [ReadOnly] public int score = 0;
    public int pointGoodCategory = 5;
    public int pointGoodGame = 10;

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

    public System.Action<Client> onNextClientAction;
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
        if (_currentClient.favoriteCard == gameCard) 
        {
            score += pointGoodGame;
        }
        else
        {
            foreach (GameGenre genre in gameCard.genres)
            {
                if (_currentClient.genrePreference.Contains(genre))
                {
                    score += pointGoodCategory;
                    return;
                }
            }
        }
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
}
