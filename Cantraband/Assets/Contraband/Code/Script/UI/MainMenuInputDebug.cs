using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class MainMenuInputDebug : MonoBehaviour
{
    private const string CARD_INSERTED_MESSAGE = "Card inserted :";

    [Header("References")]
    [SerializeField] private TMP_Text _debugText;

    [Header("Parameter")]
    [SerializeField] private float _inputTextDelay;

    private Coroutine _inputTextCoroutine;

    private void OnEnable()
    {
        _debugText.text = string.Empty;
        InputManager.Instance.OnReadCard += ShowDebugInput;
    }

    private void OnDisable()
    {
        InputManager.Instance.OnReadCard -= ShowDebugInput;
    }

    private void ShowDebugInput(string cardTag)
    {
        if(_inputTextCoroutine != null)
        {
            StopCoroutine(_inputTextCoroutine);
            _inputTextCoroutine = null;
        }
        
        string cardName = "Unknown";
        foreach(GameCard gameCard in DataContainer.GameCards)
        {
            if(gameCard.tag == cardTag || gameCard.DebugKeyboardTag == cardTag)
            {
                cardName = gameCard.name;
                break;
            }
        }

        _inputTextCoroutine = StartCoroutine(DebugTextDelay(cardName));
    }

    private IEnumerator DebugTextDelay(string cardName)
    {
        _debugText.text = CARD_INSERTED_MESSAGE + cardName;
        yield return new WaitForSeconds(_inputTextDelay);
        _debugText.text = string.Empty;
    }
}
