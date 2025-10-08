using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance;

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

        IsVestOpened = false;
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
            SetVestState(true);
        }
        else if(Keyboard.current[Key.C].wasReleasedThisFrame)
        {
            SetVestState(false);
        }

        if (Keyboard.current[Key.Digit1].wasPressedThisFrame)
        {
            ReceiveNFCReader("Test1");
        }
        if (Keyboard.current[Key.Digit2].wasPressedThisFrame)
        {
            ReceiveNFCReader("Test2");
        }
    }
}
