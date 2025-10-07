using System;
using System.Collections.Generic;
using Unity.Android.Gradle.Manifest;
using UnityEngine;
using UnityEngine.Events;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }
    private Dictionary<int, GameCard> _gameCards;
    private List<Client> _clients;
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
        if (_currentClient.favoriteCard == gameCard) 
        {
            _score += pointGoodGame;
        }
        else
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
}
