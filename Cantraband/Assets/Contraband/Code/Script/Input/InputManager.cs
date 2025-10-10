using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance;

    [SerializeField] bool _defaultVestOpened;

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

    private void SetVestState(bool state)
    {
        Debug.Log("Input Vest State is: " + state.ToString());

        IsVestOpened = state;
        OnVestChanged?.Invoke(state);
    }


    private void Update()
    {
        KeyboardInputJoueur();
    }

    private void KeyboardInputJoueur()
    {
        if (Keyboard.current[Key.O].wasPressedThisFrame)
        {
            SetVestState(false);
        }
        else if(Keyboard.current[Key.O].wasReleasedThisFrame)
        {
            SetVestState(true);
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
