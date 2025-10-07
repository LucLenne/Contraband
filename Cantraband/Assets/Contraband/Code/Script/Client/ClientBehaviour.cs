using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ClientBehaviour : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _spriteRendererClient;
    [Header("UI"),SerializeField] private TMP_Text _speechTextMeshPro;
    [SerializeField] private Image _imageGameCard;

    private void OnEnable()
    {
        LevelManager.Instance.onNextClientAction += LoadClient;
    }

    private void OnDisable()
    {
        LevelManager.Instance.onNextClientAction -= LoadClient;
    }


    private void LoadClient(Client client)
    {
        _spriteRendererClient.sprite = client.client;
        _speechTextMeshPro.text = client.text;
        _imageGameCard.sprite = client.clue;
    }
}
