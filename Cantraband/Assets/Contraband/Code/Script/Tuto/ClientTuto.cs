using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class ClientTuto : MonoBehaviour
{
    [Header("UI"), SerializeField] private Image _clueSprite;
    [SerializeField] private Transform _transformClient;
    [SerializeField] private Slider _sliderPatience;

    [Header("Gameplay"), SerializeField] private int _timePatience = 10;

    [Header("References"), SerializeField] private GameObject _sliderGO;

    public GameObject InitClient(Client client, GameCard card)
    {
        //_clueSprite.sprite = card.hintImage;
        return Instantiate(client.clientPrefab, _transformClient);
    }

    public async void ActiveTimer()
    {
        _sliderGO.SetActive(true);
        await StartTimerPatience();
    }

    private async Task StartTimerPatience()
    {
        float timeLeft = _timePatience;
        _sliderPatience.maxValue = timeLeft;
        _sliderPatience.value = timeLeft;

        while (timeLeft > 0f)
        {
            await Task.Yield(); // équivalent à coroutine `yield return null`

            timeLeft -= Time.deltaTime;
            if (_sliderPatience != null)
                _sliderPatience.value = timeLeft;
        }
        _sliderPatience.value = 0f;
    }








}
