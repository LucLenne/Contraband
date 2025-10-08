using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;

public class TutoManager : MonoBehaviour
{
    [SerializeField] private Baron _baron;
    [SerializeField] private ClientTuto _clientTuto;
    private GameObject _currentClient;

    [Header("3 Clients"), SerializeField] private List<Client> _listClient;
    [Header("3 GameCards"), SerializeField] private List<GameCard> _listGameCard;
    [Header("References"), SerializeField] private GameObject _popUp;
    [SerializeField] private GameObject _policePatrol;

    private int _currentState;
    [ReadOnly] public StateTuto stateTuto = StateTuto.baron;
    private const string NAME_SCENE_GAME = "Game";

    public enum StateTuto
    {
        first,
        second,
        third,
        end,
        baron
    }

    private void Start()
    {
        CheckState();
    }

    private void OnEnable()
    {
        InputManager.Instance.OnReadCard += CheckCard;
    }

    private void OnDisable()
    {
        InputManager.Instance.OnReadCard -= CheckCard;
    }

    private void SpawnClient()
    {
        _currentClient = _clientTuto.InitClient(_listClient[(int)stateTuto], _listGameCard[(int)stateTuto]);
    }
    private  void DestroyClient()
    {
        if (_currentClient != null)
        {
            Destroy(_currentClient);
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

    private void AddTimer()
    {
        _clientTuto.ActiveTimer();
    }

    private async void CheckState()
    {
        switch (stateTuto)
        {
            case StateTuto.baron:
                
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
