using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClientTuto : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Image _sliderImage;
    [SerializeField] private Gradient _sliderGradient;
    [SerializeField] private RevealType _hintRevealType;

    [Header("Gameplay"), SerializeField] private int _timePatience = 10;
    public bool activeTimer;

    [Header("Timer slider")]
    [SerializeField] private RectTransform _timerRectTransform;
    [SerializeField] private float _timerAnimMaxSpeed;
    [SerializeField] private AnimationCurve _timerAnimCurve;

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
        foreach (Sprite hintImage in hintImages)
        {
            GameObject hint = Instantiate(_hintImagePrefab, _hintImageParent);
            Image image = hint.GetComponent<Image>();
            image.sprite = hintImage;

            Color c = image.color;
            c.a = 0;
            image.color = c;

            hint.GetComponent<SlowRevealImage>()?.CallForReveal?.Invoke(_timePatience, _hintRevealType);

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

        if (TutoManager.Instance.timerCoroutine != null)
        {
            StopCoroutine(TutoManager.Instance.timerCoroutine);
        }

        TutoManager.Instance.timerCoroutine = StartCoroutine(StartTimerAsync());
    }

    public void StopTimer()
    {
        if (TutoManager.Instance.timerCoroutine != null)
        {
            StopCoroutine(TutoManager.Instance.timerCoroutine);
            TutoManager.Instance.timerCoroutine = null;
        }
        activeTimer = false;
    }



    private IEnumerator StartTimerAsync()
    {
        float timeLeft = _timePatience;
        _sliderImage.fillAmount = timeLeft;

        Vector3 baseScale = _timerRectTransform.localScale;
        _timerRectTransform.gameObject.SetActive(true);
        while (timeLeft > 0f)
        {
            yield return null;

            timeLeft -= Time.deltaTime;

            float normalizedTime = 1 - (timeLeft / _timePatience); // 0 - 1
            float currentSpeed = _timerAnimCurve.Evaluate(normalizedTime) * _timerAnimMaxSpeed;
            float scaleFactor = 1f + Mathf.Sin(Time.time * currentSpeed * Mathf.PI) * 0.1f;

            _timerRectTransform.localScale = baseScale * scaleFactor;
            _sliderImage.color = _sliderGradient.Evaluate(normalizedTime);
            _sliderImage.fillAmount = 1 - normalizedTime;
        }
        _timerRectTransform.localScale = baseScale;
        _timerRectTransform.gameObject.SetActive(false);

        _sliderImage.fillAmount = 0f;
        TutoManager.Instance.clientEndPatience?.Invoke();
    }
}
