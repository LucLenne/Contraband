using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GDFeedbackScript : MonoBehaviour
{
    [Header("Manteau")]
    [SerializeField] private Image _manteauImage;
    [SerializeField] private Sprite _manteauCloseImage;
    [SerializeField] private Sprite _manteauOpenImage;
    [SerializeField] private AnimationCurve _manteauScaleCurve;
    [SerializeField] private float _manteauScaleDuration;
    [SerializeField] private float _manteauScaleMaxScale;

    [Header("Feedback")]
    [SerializeField] private float _feedBackFadeImageDuration;
    [SerializeField] private Image _feedbackImage;
    [SerializeField] private Color _goodCardColor;
    [SerializeField] private Color _badCardColor;

    private Coroutine _manteauScaleCoroutine;

    private void OnEnable()
    {
        InputManager.Instance.OnVestChanged += ChangeManteauImage;
        if (LevelManager.Instance != null)
            LevelManager.Instance.OnGameReturned += StartFeedbackImage;
    }

    private void OnDisable()
    {
        InputManager.Instance.OnVestChanged -= ChangeManteauImage;
        if (LevelManager.Instance != null)
            LevelManager.Instance.OnGameReturned -= StartFeedbackImage;
    }

    private void ChangeManteauImage(bool isOpened)
    {
        _manteauImage.sprite = isOpened ? _manteauOpenImage : _manteauCloseImage;
        
        if(_manteauScaleCoroutine != null)
        {
            StopCoroutine(_manteauScaleCoroutine);
            _manteauScaleCoroutine = null;
        }
        _manteauScaleCoroutine = StartCoroutine(ManteauScale());
    }

    private void StartFeedbackImage(LevelManager.GameReturnedType type)
    {
        switch (type)
        {
            case LevelManager.GameReturnedType.Favorite:
            case LevelManager.GameReturnedType.Good:
                _feedbackImage.color = _goodCardColor;
                break;

            case LevelManager.GameReturnedType.Wrong:
            default:
                _feedbackImage.color = _badCardColor;
                break;

        }

        _feedbackImage.DOFade(0, _feedBackFadeImageDuration);
    }
    private IEnumerator ManteauScale()
    {
        float timeElapsed = 0.0f;
        while (timeElapsed < _manteauScaleDuration)
        {
            float progress = timeElapsed / _manteauScaleDuration;
            float newScale = Mathf.Lerp(_manteauScaleMaxScale,1f,_manteauScaleCurve.Evaluate(progress));
            _manteauImage.transform.localScale = new Vector3(newScale, newScale, newScale);

            timeElapsed += Time.deltaTime;
            yield return null;
        }
    }
}
