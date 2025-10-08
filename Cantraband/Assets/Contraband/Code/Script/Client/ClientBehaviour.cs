using NaughtyAttributes;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class ClientBehaviour : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Animator _animator;
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
        LevelManager.Instance.OnFinishTransaction += HideClient;

        //A changer avec l'accélération du rythme
        currentPatientPatience = Mathf.Max(RythmManager.Instance.ClientPatience, .1f);
        StartTimerAsync();
    }

    private void OnDisable()
    {
        LevelManager.Instance.OnFinishTransaction -= HideClient;
    }

    private void HideClient()
    {
    }

    public async void StartTimerAsync()
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
