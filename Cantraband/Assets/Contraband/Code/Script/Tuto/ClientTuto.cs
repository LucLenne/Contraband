using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClientTuto : MonoBehaviour
{
    [ Header("UI"), SerializeField] private Slider _sliderPatience;

    [Header("Gameplay"), SerializeField] private int _timePatience = 10;
    public bool activeTimer;

    [Header("References"), SerializeField] private GameObject _sliderGO;
    [SerializeField]private GameObject _hintImagePrefab;
    [SerializeField]private Transform _hintImageParent;

    private void Start()
    {
        UnlockTimer();
    }

    public void SetupClient(List<Sprite> hintImages)
    {
        if (hintImages == null || hintImages.Count == 0)
        {
            Debug.LogWarning("No hint images provided for this client.");
            return;
        }

        foreach (Sprite hintImage in hintImages)
        {
            GameObject newGO = Instantiate(_hintImagePrefab, _hintImageParent);
            newGO.GetComponent<Image>().sprite = hintImage;
        }
    }

    public void InitClient(Client client, GameCard card)
    {
        SetupClient(card.hintImage);

        if (!activeTimer)
            _sliderGO.SetActive(false);
        else
            ActiveTimer(); // si activeTimer est déjà true (déjà décidé par TutoManager)
    }

    public void InitTimer()
    {
        activeTimer = true;
        ActiveTimer();
    }

    void UnlockTimer()
    {
        
        if (activeTimer)
        {
            ActiveTimer();
        }
            
    }

    private void ActiveTimer()
    {
        _sliderGO.SetActive(true);
        StartCoroutine(StartTimerPatience());
    }

    private IEnumerator StartTimerPatience()
    {
        float timeLeft = _timePatience;
        _sliderPatience.maxValue = timeLeft;
        _sliderPatience.value = timeLeft;

        while (timeLeft > 0f)
        {
            yield return null; // équivalent de Task.Yield() dans Unity Coroutine

            timeLeft -= Time.deltaTime;
            if (_sliderPatience != null)
                _sliderPatience.value = timeLeft;
        }

        _sliderPatience.value = 0f;
        TutoManager.Instance.clientEndPatience?.Invoke();
    }

}
