using NaughtyAttributes;
using System;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;

public class TutoManager : MonoBehaviour
{
    public static TutoManager Instance;


    [Header("GamePlay"), ReadOnly] public StateTuto stateTuto;
    [Header("Put the card and the client in the right order")]
    [Header("3 Clients"), SerializeField] private List<Client> _listClient;
    [Header("3 GameCards"), SerializeField] private List<GameCard> _listGameCard;

    [Header("References"), SerializeField] private GameObject _popUp;
    [SerializeField] private GameObject _policePatrol;
    [SerializeField] private GameObject _prefabTutoClient;
    [SerializeField] private Baron _baron;
    public Transform posClient;

    private GameObject _currentClient;
    private ClientTuto _clientTuto;
    private int _currentState;

    private bool _activePopUp;
    private bool _activeTimer;
    
    private const string NAME_SCENE_GAME = "Game";

    public Action clientEndPatience;

    public enum StateTuto
    {
        first,
        second,
        third,
        end,
        baron
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        stateTuto = StateTuto.baron;
        CheckState();
    }

    private void OnEnable()
    {
        InputManager.Instance.OnReadCard += CheckCard;
        clientEndPatience += ChangeClient;
        
    }

    private void OnDisable()
    {
        InputManager.Instance.OnReadCard -= CheckCard;
        clientEndPatience -= ChangeClient;
    }

    private void SpawnClient()
    {
        GameObject client = _prefabTutoClient;
        ClientTuto clientTuto = client.GetComponent<ClientTuto>();
        if (_activeTimer)
        {
            clientTuto.activeTimer = true;
        }
        clientTuto.InitClient(_listClient[_currentState], _listGameCard[_currentState]);
        _currentClient = Instantiate(client,posClient);
        _clientTuto = clientTuto;
    }

    private  void DestroyClient()
    {
        if (_currentClient != null)
        {
            Destroy(_currentClient);
            _clientTuto = null;
        }
    }
    private void ChangeClient()
    {
        DestroyClient();
        SpawnClient();
    }


    private void AddPopUp()
    {
        _policePatrol.gameObject.SetActive(true);
        _popUp.gameObject.SetActive(true);
    }

    private void AddTimer() => _clientTuto.activeTimer = true;

    private async void CheckState()
    {
        switch (stateTuto)
        {
            case StateTuto.baron:
                Debug.Log("in baron");
                await _baron.SpeechBaron(_currentState);
                stateTuto = (StateTuto)_currentState;
                ChangeClient();
                break;
            case StateTuto.first:
                break;
            case StateTuto.second:
                break;
            case StateTuto.third:
                AddPopUp();
                AddTimer();
                break;
            case StateTuto.end:
                LoadingManager.Instance.LoadScene(NAME_SCENE_GAME);
                break;
        }
    }

    private void CheckCard(string card)
    {
        if(card == _listGameCard[(int)stateTuto].tag)
        {
            _currentState += 1;
            stateTuto = StateTuto.baron;
            CheckState();
        }
    }
}
