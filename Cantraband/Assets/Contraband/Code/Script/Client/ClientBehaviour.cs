using NaughtyAttributes;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ClientBehaviour : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SpriteRenderer _spriteRendererClient;
    [SerializeField] private GameObject _canvasObject;

    [Header("UI"), SerializeField] private Image _imageGameCard;

    [Header("Patience")]
    [SerializeField] private float _baseClientPatience;
    [ReadOnly] public float currentPatientPatience;

    private Coroutine _patienceCoroutine;

    private void Awake()
    {
        HideClient();
    }

    private void OnEnable()
    {
        LevelManager.Instance.onNextClientAction += LoadClient;
        LevelManager.Instance.OnFinishTransaction += HideClient;
    }

    private void OnDisable()
    {
        LevelManager.Instance.onNextClientAction -= LoadClient;
        LevelManager.Instance.OnFinishTransaction -= HideClient;
    }

    private void HideClient()
    {
        Debug.LogWarning("A changer pour mettre une anim à la place");
        _spriteRendererClient.enabled = false;
        _canvasObject.SetActive(false);

        if(_patienceCoroutine != null)
        {
            StopCoroutine(_patienceCoroutine);
            _patienceCoroutine = null;
        }
    }

    private void LoadClient(Client client)
    {
        _canvasObject.SetActive(true);
        _spriteRendererClient.enabled = true;
        
        //A changer avec l'accélération du rythme
        currentPatientPatience = _baseClientPatience;
        _patienceCoroutine = StartCoroutine(PatienceRoutine());

        _spriteRendererClient.sprite = client.client;
        _imageGameCard.sprite = client.favoriteCard.hintImage;
    }

    private IEnumerator PatienceRoutine()
    {
        yield return new WaitForSeconds(_baseClientPatience);
        LevelManager.Instance.ClientNoMorePatience();
    }

}
