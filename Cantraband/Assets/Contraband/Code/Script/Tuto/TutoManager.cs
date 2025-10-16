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
    [SerializeField] private float _timeBeforeDestroyClient = 1f;
    [SerializeField] private float _timeBetweenPopUpPolice = 5;
    [SerializeField] private float _timeTransitionGameplayBaron = 2f;
    [SerializeField] private int _pointGoodCard = 3;
    [SerializeField] private int _pointGoodTheme = 1;
    [SerializeField] private int _pointBadCard = -1;
    [Header("Put the card and the client in the right order")]
    [Header("3 Clients"), SerializeField] private List<Client> _listClient;

    [Header("References"), SerializeField] private GameObject _popUp;
    [SerializeField] private GameObject _policePatrol;
    [SerializeField] private GameObject _prefabTutoClient;
    [SerializeField] private Baron _baron;
    [SerializeField] GDFeedbackScript _gdFeedBackScript;
    public Transform posClient;

    private GameObject _currentClient;
    private ClientTuto _clientTuto;
    private int _currentState;
    private bool _isHandlingOutOfPatience = false;
    public Coroutine timerCoroutine;
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
        StartCoroutine(CheckState());
    }

    private void OnEnable()
    {
        InputManager.Instance.OnReadCard += CheckCard;
        clientEndPatience += ClientOutOfPatience;

    }

    private void OnDisable()
    {
        InputManager.Instance.OnReadCard -= CheckCard;
        clientEndPatience -= ClientOutOfPatience;
    }

    private void SpawnClient()
    {
        _currentClient = Instantiate(_prefabTutoClient, posClient);
        _clientTuto = _currentClient.GetComponent<ClientTuto>();

        _clientTuto.InitClient(_listClient[_currentState], _listClient[_currentState].favoriteCard);
    }

    private void DestroyClient()
    {
        if (_clientTuto != null)
        {
            _clientTuto.StopTimer();
        }
        StartCoroutine(CoroutineDestroyClient());
    }

    private IEnumerator CoroutineDestroyClient()
    {
        yield return new WaitForSeconds(_timeBeforeDestroyClient);
        if (_currentClient != null)
        {
            Destroy(_currentClient);
            _clientTuto = null;
        }
    }

    private IEnumerator CoroutineOutOfPatience()
    {
        _clientTuto.LaunchOutOfPatience();
        yield return new WaitForSeconds(2f);
        yield return StartCoroutine(CoroutineDestroyClient());
        SpawnClient();
        _clientTuto.activeTimer = true;
        _clientTuto.InitTimer();
        _isHandlingOutOfPatience = false;
    }


    private void ClientOutOfPatience()
    {
        if (_isHandlingOutOfPatience) return;

        _isHandlingOutOfPatience = true;
        StartCoroutine(CoroutineOutOfPatience());
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

    private void CheckCard(string card)
    {
        Debug.Log("card input");
        if (stateTuto != StateTuto.baron && _clientTuto != null)
        {
            Debug.Log("client tuto not null & state != baron");
            if (card == _listClient[(int)stateTuto].favoriteCard.tag || card == _listClient[(int)stateTuto].favoriteCard.DebugKeyboardTag)
            {
                Debug.Log("good card");
                _gdFeedBackScript.StartFeedbackImage(LevelManager.GameReturnedType.Good);
                _clientTuto.LaunchTransferDoneAnim();
                score += _pointGoodCard;
                onClientLeave?.Invoke();
                _currentState += 1;
                stateTuto = StateTuto.baron;
                StartCoroutine(CheckState());
            }
            else
            {
                _gdFeedBackScript.StartFeedbackImage(LevelManager.GameReturnedType.Wrong);
                Debug.Log("bad card");
            }
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
        yield return StartCoroutine(_baron.SpeechBaronCoroutine(_currentState, _timeTransitionGameplayBaron));
        if (_currentState == 3)
        {
            stateTuto = StateTuto.end;
        }
        else
        {
            stateTuto = (StateTuto)_currentState;
            SpawnClient();
        }
        StartCoroutine(CheckState());
    }


    public IEnumerator CheckState()
    {
        switch (stateTuto)
        {
            case StateTuto.baron:
                DestroyClient();
                if (_currentState == 3)
                {
                    _policePatrol.gameObject.SetActive(false);
                    _popUp.gameObject.SetActive(false);
                    PopUpManager.Instance.gameObject.SetActive(false);
                }
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
