using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class GDFeedbackScript : MonoBehaviour
{
    [Header("Manteau")]
    [SerializeField] private Image _manteauImage;
    [SerializeField] private Sprite _manteauCloseImage;
    [SerializeField] private Sprite _manteauOpenImage;

    [Header("Feedback")]
    [SerializeField] private float _feedBackFadeImageDuration;
    [SerializeField] private Image _feedbackImage;
    [SerializeField] private Color _goodCardColor;
    [SerializeField] private Color _badCardColor;

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

    private void ChangeManteauImage(bool isOpened) => _manteauImage.sprite = isOpened ? _manteauOpenImage : _manteauCloseImage;

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
}
