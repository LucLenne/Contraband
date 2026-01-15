using System;
using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance;

    [SerializeField] bool _defaultVestOpened;
    [Space]
    [SerializeField] private float _openVestDelay = .2f;

    private Coroutine _openVestCoroutine;

    public Action<string> OnReadCard;
    public Action<bool> OnVestChanged;

    public bool IsVestOpened { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        /*        OnReadCard += GetNFCReader;
                OnVestChanged += GetVestState;*/

        //Set default vest state
        IsVestOpened = _defaultVestOpened;
        SendFirstOnVestChanged();
    }

    private void OnDisable()
    {
        StopOpenVestCoroutine();
    }

    private async void SendFirstOnVestChanged()
    {
        await Task.Delay(10);
        OnVestChanged?.Invoke(_defaultVestOpened);
    }

    public void ReceiveNFCReader(string cardId)
    {
        Debug.Log("Input NFC Value is: " + cardId);
        OnReadCard?.Invoke(cardId);
    }

    private IEnumerator SetVestState(bool state)
    {
        Debug.Log("Input Vest State is: " + state.ToString());

        //Si fermé -> alors maintient
        if(!state)
        {
            IsVestOpened = false;
            OnVestChanged?.Invoke(false);
            yield break;
        }

        //Sinon, delai puis ouvre
        //Vu que c'est une coroutine, elle s'arrêtera si le joueur la referme
        yield return new WaitForSeconds(_openVestDelay);

        IsVestOpened = true;
        OnVestChanged?.Invoke(true);
    }


    private void Update()
    {
        KeyboardInputJoueur();
    }

    private void KeyboardInputJoueur()
    {
        //if (Input.inputString != "") Debug.Log(Input.inputString);

        if (Joystick.current != null)
        {
            var buttons = Joystick.current.allControls.OfType<ButtonControl>().ToArray();
            if (buttons.Length > 0 && buttons[0].wasPressedThisFrame)
            {
                StopOpenVestCoroutine();
                _openVestCoroutine = StartCoroutine(SetVestState(false));
            } else if (buttons.Length > 0 && buttons[0].wasReleasedThisFrame)
            {
                StopOpenVestCoroutine();
                _openVestCoroutine = StartCoroutine(SetVestState(true));
            }

        }
        if (Keyboard.current != null)
        {
            if (Keyboard.current[Key.O].wasPressedThisFrame)
            {
                StopOpenVestCoroutine();
                _openVestCoroutine = StartCoroutine(SetVestState(false));
            }
            else if (Keyboard.current[Key.O].wasReleasedThisFrame)
            {
                StopOpenVestCoroutine();
                _openVestCoroutine = StartCoroutine(SetVestState(true));
            }

            if (Keyboard.current[Key.Digit1].wasPressedThisFrame)
            {
                ReceiveNFCReader("Test1");
            }
            if (Keyboard.current[Key.Digit2].wasPressedThisFrame)
            {
                ReceiveNFCReader("Test2");
            }
            if (Keyboard.current[Key.Digit3].wasPressedThisFrame)
            {
                ReceiveNFCReader("Test3");
            }
            if (Keyboard.current[Key.Digit4].wasPressedThisFrame)
            {
                ReceiveNFCReader("Test4");
            }
        }
    }

    private void StopOpenVestCoroutine()
    {
        if (_openVestCoroutine != null)
        {
            StopCoroutine(_openVestCoroutine);
            _openVestCoroutine = null;
        }
    }
}
