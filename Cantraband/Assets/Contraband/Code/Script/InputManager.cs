using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance;

    public Action<int> OnReadCard;
    public Action<bool> OnVestChanged;
    public bool DebugGetKeyboardInput = true;

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

    private void GetNFCReader(int cardId)
    {
        Debug.Log("NFC Value is: " + cardId);
        OnReadCard?.Invoke(cardId);
    }

    private void SetVestState(bool state)
    {
        Debug.Log("Vest State is: " + state.ToString());

        IsVestOpened = state;
        OnVestChanged?.Invoke(state);
    }


    private void Update()
    {
        if (DebugGetKeyboardInput)
            KeyboardInputJoueur();
    }

    private void KeyboardInputJoueur()
    {
        if (Keyboard.current[Key.O].wasPressedThisFrame)
        {
            SetVestState(true);
        }
        if (Keyboard.current[Key.C].wasPressedThisFrame)
        {
            SetVestState(false);
        }
        if (Keyboard.current[Key.Digit1].wasPressedThisFrame)
        {
            GetNFCReader(1);
        }
        if (Keyboard.current[Key.Digit2].wasPressedThisFrame)
        {
            GetNFCReader(2);
        }
    }
}
