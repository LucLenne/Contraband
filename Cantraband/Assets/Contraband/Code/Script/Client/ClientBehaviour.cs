using NaughtyAttributes;
using System.Collections;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ClientBehaviour : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SpriteRenderer _spriteRendererClient;
    [SerializeField] private GameObject _canvasObject;

    [Header("UI"), SerializeField] private Image _imageGameCard;
    [SerializeField]private Slider _sliderPatience;

    [Header("Patience")]
    [ReadOnly] public float currentPatientPatience;
    private bool _newClient = false;

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
        _newClient = true;
        _canvasObject.SetActive(false);

        
    }

    private async void LoadClient(Client client)
    {
        _newClient = false;
        _canvasObject.SetActive(true);
        _spriteRendererClient.enabled = true;
        _spriteRendererClient.sprite = client.client;
        _imageGameCard.sprite = client.favoriteCard.hintImage;

        //A changer avec l'accélération du rythme
        currentPatientPatience = Mathf.Max(RythmManager.Instance.ClientPatience, .1f);
        await StartTimerAsync();
    }


    public async Task StartTimerAsync()
    {
        float timeLeft =  currentPatientPatience;
        _sliderPatience.maxValue = timeLeft;
        _sliderPatience.value = timeLeft;

        while (timeLeft > 0f && !_newClient)
        {
            await Task.Yield(); // équivalent à coroutine `yield return null`

            timeLeft -= Time.deltaTime;
            if(_sliderPatience != null)
                _sliderPatience.value = timeLeft;
        }
        _sliderPatience.value = 0f;
        LevelManager.Instance.ClientNoMorePatience();
    }

}
