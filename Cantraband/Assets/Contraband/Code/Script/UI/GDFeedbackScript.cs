using System;
using UnityEngine;
using UnityEngine.UI;

public class GDFeedbackScript : MonoBehaviour
{
    [Header("Manteau")]
    [SerializeField] private Image _manteauImage;
    [SerializeField] private Sprite _manteauCloseImage;
    [SerializeField] private Sprite _manteauOpenImage;

    private void OnEnable()
    {
        InputManager.Instance.OnVestChanged += ChangeManteauImage;
    }

    private void OnDisable()
    {
        InputManager.Instance.OnVestChanged -= ChangeManteauImage;
    }

    private void ChangeManteauImage(bool isOpened) => _manteauImage.sprite = isOpened ? _manteauOpenImage : _manteauCloseImage;
}
