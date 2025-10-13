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
    [SerializeField] private float _timeBetweenClient;
    [SerializeField] private float _timeBetweenPopUpPolice = 5;
    [SerializeField] private float _timeTransitionGameplayBaron = 2f;
    [SerializeField] private int _pointGoodCard = 3;
    [SerializeField] private int _pointGoodTheme = 1;
    [SerializeField] private int _pointBadCard = -1;
    [Header("Put the card and the client in the right order")]
    [Header("3 Clients"), SerializeField] private List<Client> _listClient;
    [Header("3 GameCards"), SerializeField] private List<GameCard> _listGameCard;

    [Header("References"), SerializeField] private GameObject _popUp;
    [SerializeField] private GameObject _policePatrol;
    [SerializeField] private GameObject _prefabTutoClient;
    [SerializeField] private Baron _baron;
    [SerializeField] GDFeedbackScript _gdFeedBackScript;
    public Transform posClient;

    private GameObject _currentClient;
    private ClientTuto _clientTuto;
    private int _currentState;
    [HideInInspector] public int score;

    private const string NAME_SCENE_GAME = "Game";

    public Action clientEndPatience;
    public Action onClientLeave;

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
        SpawnClient();
        StartCoroutine(CheckState());
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
        StartCoroutine(CoroutineChangeClient());
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

    public void CheckPlayerCoat()
    {
        if (!InputManager.Instance.IsVestOpened)
            return;
    }

    private void CheckCard(string card)
    {
        if (stateTuto != StateTuto.baron)
        {
            if (card == _listGameCard[(int)stateTuto].tag || _listGameCard[(int)stateTuto].DebugKeyboardTag == card)
            {
                score += _pointGoodCard;
                onClientLeave?.Invoke();
                _currentState += 1;
                stateTuto = StateTuto.baron;
                StartCoroutine(CheckState());
            }
            else
            {
                _clientTuto.LaunchOutOfPatience();
            }
        }
    }

    IEnumerator CoroutineChangeClient()
    {
        DestroyClient();
        yield return new WaitForSeconds(_timeBetweenClient);
        SpawnClient();

        if (stateTuto == StateTuto.third && _clientTuto != null)
        {
            _clientTuto.activeTimer = true;
            _clientTuto.InitTimer();
        }
    }

    private IEnumerator TimerBetweenPopupPolice()
    {
        _popUp.GetComponent<PopUpManager>().SpawnRandomPolicePopup();
        yield return new WaitForSeconds(_timeBetweenPopUpPolice);
        StartCoroutine(TimerBetweenPopupPolice());
    }

    private IEnumerator HandleBaronState()
    {
        _clientTuto.LaunchTransferDoneAnim();
        _gdFeedBackScript.StartFeedbackImage(LevelManager.GameReturnedType.Favorite);
        yield return StartCoroutine(_baron.SpeechBaronCoroutine(_currentState, _timeTransitionGameplayBaron));
        if (_currentState == 3)
        {
            stateTuto = StateTuto.end;
            StartCoroutine(CheckState());
        }
        else
        {
            stateTuto = (StateTuto)_currentState;
            ChangeClient();
            StartCoroutine(CheckState());
        }


    }


    public IEnumerator CheckState()
    {
        switch (stateTuto)
        {
            case StateTuto.baron:
                yield return StartCoroutine(HandleBaronState());
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
}
