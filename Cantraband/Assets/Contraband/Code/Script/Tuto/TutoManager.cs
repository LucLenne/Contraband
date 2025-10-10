using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
        _currentClient = Instantiate(_prefabTutoClient, posClient);
        _clientTuto = _currentClient.GetComponent<ClientTuto>();

        _clientTuto.InitClient(_listClient[_currentState], _listGameCard[_currentState]);
    }



    private void DestroyClient()
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

        // Si on est dans le step 3 on active le timer pour le nouveau client
        if (stateTuto == StateTuto.third && _clientTuto != null)
        {
            _clientTuto.activeTimer = true;
            _clientTuto.InitTimer(); //déclenche le compte à rebours
        }
    }



    void StartPatrol()
    {
        _policePatrol.GetComponent<PolicePatrol>().ActivatePatrol();
    }

    private void AddPopUp()
    {
        _popUp.gameObject.SetActive(true);
        StartCoroutine(TimerBetweenPopupPolice());
        _policePatrol.gameObject.SetActive(true);
    }

    private IEnumerator TimerBetweenPopupPolice()
    {
        _popUp.GetComponent<PopUpManager>().SpawnRandomPolicePopup();
        yield return new WaitForSeconds(5f);
        StartCoroutine(TimerBetweenPopupPolice());
    }

    private void AddTimer() => _clientTuto.activeTimer = true;

    private IEnumerator HandleBaronState()
    {
        yield return StartCoroutine(_baron.SpeechBaronCoroutine(_currentState));
        stateTuto = (StateTuto)_currentState;
        ChangeClient();
        CheckState();
    }


    private void CheckState()
    {
        Debug.Log(stateTuto.ToString());

        switch (stateTuto)
        {
            case StateTuto.baron:
                StartCoroutine(HandleBaronState());
                break;

            case StateTuto.first:
                break;

            case StateTuto.second:
                break;

            case StateTuto.third:
                AddPopUp();
                StartPatrol();

                if (_clientTuto != null)
                {
                    _clientTuto.activeTimer = true;
                    _clientTuto.InitTimer();
                }
                break;

            case StateTuto.end:
                if (LoadingManager.Instance != null)
                {
                    LoadingManager.Instance.LoadScene(NAME_SCENE_GAME);
                }
                else
                {
                    SceneManager.LoadScene(NAME_SCENE_GAME);
                }

                break;
        }
    }
    public void CheckPlayerCoat()
    {
        if (!InputManager.Instance.IsVestOpened)
            return;
    }

    private void CheckCard(string card)
    {
        Debug.Log(card);
        Debug.Log("Debug Keyboard Tag : " + _listGameCard[(int)stateTuto].DebugKeyboardTag);
        if (card == _listGameCard[(int)stateTuto].tag || _listGameCard[(int)stateTuto].DebugKeyboardTag == card)
        {
            _currentState += 1;
            stateTuto = StateTuto.baron;
            if (_currentState == 3)
                stateTuto = StateTuto.end;
            CheckState();
        }
    }
}
