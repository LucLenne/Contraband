using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }
    private Dictionary<int, GameCard> _gameCards;
    private List<Client> _clients;
    private Client _currentClient;
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


    GameCard GetCardGame(int tag)
    {
        return _gameCards[tag];
    }

    Client GetClient()
    {
        return new();
    }

    void NextClient()
    {
        _currentClient = GetRandomClient();
    }

    Client GetRandomClient()
    {
        if (_clients.Count == 0) return null;

        int randomIndex = Random.Range(0, _clients.Count);
        return _clients[randomIndex];
    }
}
