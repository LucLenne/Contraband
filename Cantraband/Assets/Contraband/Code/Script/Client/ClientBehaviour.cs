using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ClientBehaviour : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SpriteRenderer _spriteRendererClient;
    [SerializeField] private GameObject _canvasObject;

    [Header("UI"),SerializeField] private TMP_Text _speechTextMeshPro;
    [SerializeField] private Image _imageGameCard;

    private void Awake()
    {
        HideClient();
    }

    private void OnEnable()
    {
        LevelManager.Instance.onNextClientAction += LoadClient;
        LevelManager.Instance.OnValidateTransaction += HideClient;
    }

    private void OnDisable()
    {
        LevelManager.Instance.onNextClientAction -= LoadClient;
        LevelManager.Instance.OnValidateTransaction -= HideClient;
    }

    private void HideClient()
    {
        Debug.LogWarning("A changer pour mettre une anim à la place");
        _spriteRendererClient.enabled = false;
        _canvasObject.SetActive(false);
    }

    private void LoadClient(Client client)
    {
        _canvasObject.SetActive(true);
        _spriteRendererClient.enabled = true;

        _spriteRendererClient.sprite = client.client;
        _speechTextMeshPro.text = client.text;
        _imageGameCard.sprite = client.clue;
    }
}
