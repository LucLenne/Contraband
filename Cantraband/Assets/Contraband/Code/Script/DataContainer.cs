using System.Collections.Generic;
using UnityEngine;

public class DataContainer : MonoBehaviour
{
    public static DataContainer Instance;

    [SerializeField] private List<GameCard> _gameCards;
    [SerializeField] private List<Client> _clients;

    public static List<Client> Clients { get { return Instance._clients; } }
    public static List<GameCard> GameCards { get { return Instance._gameCards; } }

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
}
