using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClientTuto : MonoBehaviour
{
    [Header("UI"), SerializeField] private Image _imagePatience;

    [Header("Gameplay"), SerializeField] private int _timePatience = 10;
    public bool activeTimer;

    [Header("References"), SerializeField] private GameObject _sliderGO;
    [SerializeField] private GameObject _hintImagePrefab;
    [SerializeField] private Transform _hintImageParent;
    [SerializeField] private Animator _animator;

    private const string ANIMATION_TRANSFER_DONE_NAME = "TransferDone";
    private const string ANIMATION_GAME_OVER_NAME = "GameOver";
    private const string ANIMATION_TRANSFER_COP_NAME = "TransferCop";
    private const string ANIMATION_OUT_OF_PATIENCE = "OutPatience";

    private void Start()
    {
        UnlockTimer();
    }
    public void LaunchCopAnim() => _animator.SetTrigger(ANIMATION_TRANSFER_COP_NAME);
    private void LaunchGameOverAnim() => _animator.SetTrigger(ANIMATION_GAME_OVER_NAME);
    public void LaunchTransferDoneAnim() => _animator.SetTrigger(ANIMATION_TRANSFER_DONE_NAME);
    public void LaunchOutOfPatience() => _animator.SetTrigger(ANIMATION_OUT_OF_PATIENCE);

    public void DestroyObject()
    {

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
        _imagePatience.fillAmount = timeLeft;

        while (timeLeft > 0f)
        {
            yield return null; // équivalent de Task.Yield() dans Unity Coroutine

            timeLeft -= Time.deltaTime;
            _imagePatience.fillAmount = timeLeft;
        }

        _imagePatience.fillAmount = 0;
        TutoManager.Instance.clientEndPatience?.Invoke();
    }

}
