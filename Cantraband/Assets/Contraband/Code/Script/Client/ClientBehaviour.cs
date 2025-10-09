using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClientBehaviour : MonoBehaviour
{
    private const string ANIMATION_TRANSFER_DONE_NAME = "TransferDone";
    private const string ANIMATION_GAME_OVER_NAME = "GameOver";
    private const string ANIMATION_TRANSFER_COP_NAME = "TransferCop";

    [Header("References")]
    [SerializeField] private Animator _animator;
    [SerializeField] private GameObject _canvasObject;

    [Header("UI")]
    [SerializeField] private GameObject _hintImagePrefab;
    [SerializeField] private RectTransform _hintImageParent;
    [SerializeField] private Slider _sliderPatience;

    [Header("Timer slider")]
    [SerializeField] private RectTransform _timerRectTransform;
    [SerializeField] private float _timerAnimMaxSpeed;
    [SerializeField] private AnimationCurve _timerAnimCurve;

    [Header("Patience")]
    [ReadOnly] public float currentPatientPatience;

    private bool _newClient = false;
    private Coroutine _newClientCoroutine;

    private void OnEnable()
    {
        LevelManager.Instance.OnFinishTransaction += LaunchTransferDoneAnim;
        LevelManager.Instance.OnFailedByCop += LaunchCopAnim;
        LevelManager.Instance.OnGameOver += LaunchGameOverAnim;

        //A changer avec l'accélération du rythme
        currentPatientPatience = Mathf.Max(RythmManager.Instance.ClientPatience, .1f);
        _newClientCoroutine = StartCoroutine(StartTimerAsync());
    }

    private void OnDisable()
    {
        LevelManager.Instance.OnFinishTransaction -= LaunchTransferDoneAnim;
        LevelManager.Instance.OnFailedByCop -= LaunchCopAnim;
        LevelManager.Instance.OnGameOver -= LaunchGameOverAnim;

        if (_newClientCoroutine != null)
        {
            StopCoroutine(_newClientCoroutine);
            _newClientCoroutine = null;
        }
    }

    public void SetupClient(List<Sprite> hintImages)
    {
        foreach (Sprite hintImage in hintImages)
        {
            Image image = Instantiate(_hintImagePrefab, _hintImageParent).GetComponent<Image>();
            image.sprite = hintImage;
        }
    }

    private void LaunchCopAnim() => _animator.SetTrigger(ANIMATION_TRANSFER_COP_NAME);
    private void LaunchGameOverAnim() => _animator.SetTrigger(ANIMATION_GAME_OVER_NAME);
    private void LaunchTransferDoneAnim() => _animator.SetTrigger(ANIMATION_TRANSFER_DONE_NAME);

    private IEnumerator StartTimerAsync()
    {
        float timeLeft = currentPatientPatience;
        _sliderPatience.maxValue = timeLeft;
        _sliderPatience.value = timeLeft;

        Vector3 baseScale = _timerRectTransform.localScale;
        _timerRectTransform.gameObject.SetActive(true);
        while (timeLeft > 0f && !_newClient)
        {
            yield return null;

            timeLeft -= Time.deltaTime;
            if (_sliderPatience != null)
                _sliderPatience.value = timeLeft;

            float normalizedTime = 1 - (timeLeft / currentPatientPatience); // 0 - 1
            float currentSpeed = _timerAnimCurve.Evaluate(normalizedTime) * _timerAnimMaxSpeed;
            float scaleFactor = 1f + Mathf.Sin(Time.time * currentSpeed * Mathf.PI) * 0.1f;
            _timerRectTransform.localScale = baseScale * scaleFactor;
        }
        _timerRectTransform.localScale = baseScale;
        _timerRectTransform.gameObject.SetActive(false);

        _sliderPatience.value = 0f;
        LevelManager.Instance.StartClientNoMorePatience();
    }

    #region Animation methods
    public void LaunchGameOver() => LevelManager.Instance.LaunchGameOver();
    public void DestroyObject() => Destroy(gameObject);
    #endregion
}
