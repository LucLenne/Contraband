using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using static PopUpManager;

public class PopUpGeneric : MonoBehaviour
{
    [System.Serializable]
    private struct AngleData
    {
        public AngleType type;
        public Sprite defaultImage;
        public Sprite OnWarningImage;
        [Space]
        public Vector2 anchorMinMax;
        public Vector2 anchoredPosition;
    }

    [Header("References")]
    [SerializeField] private RectTransform _rect;
    [SerializeField] private Animator _animator;
    [SerializeField] private Image _frameImage;
    //[SerializeField] private List<AngleData> _angleDatas;

    [Header("Parameters")]
    [SerializeField] private float _speed = 1f;

    [Header("Warning Image")]
    [SerializeField] private float _warningImageTime;

    [Header("Scale")]
    [SerializeField, MinMaxSlider(0, 10)] private Vector2 _minMaxScale;
    [SerializeField] private AnimationCurve _scaleCurve;

    [Header("Events")]
    [SerializeField] private UnityEvent _onCheckPlayerCoat;

    private Vector2 _positionAt1Scale;

    private Sprite _defaultFrameImage;
    private Sprite _warningFrameImage;

    private Coroutine _scaleFrameRoutine;
    private Coroutine _warningFrameCoroutine;

    public float Speed
    {
        get => _speed;
        set => _speed = value;
    }

    private void Awake()
    {
        _animator.speed = _speed;

        _scaleFrameRoutine = StartCoroutine(ScaleRoutine());
    }

    private void OnEnable()
    {
        LevelManager.Instance.OnGameOver += StopAnimation;
    }

    private void OnDisable()
    {
        LevelManager.Instance.OnGameOver -= StopAnimation;
    }

    #region setup
    //public void SetupPopup(AngleType angle, float speedOverride = -1)
    //{
    //    AngleData angleData = FindDataByAngleType(angle);
    //    //Setup anchors & positions
    //    _rect.anchorMin = angleData.anchorMinMax;
    //    _rect.anchorMax = angleData.anchorMinMax;
    //    _rect.anchoredPosition = angleData.anchoredPosition;
    //    _positionAt1Scale = angleData.anchoredPosition;

    //    //Setup images
    //    _defaultFrameImage = angleData.defaultImage;
    //    _frameImage.sprite = _defaultFrameImage;
    //    _warningFrameImage = angleData.OnWarningImage;

    //    //Setup speed
    //    if(speedOverride != -1)
    //        _speed = speedOverride;
    //}
    //private AngleData FindDataByAngleType(AngleType angleType)
    //{
    //    foreach (AngleData data in _angleDatas)
    //    {
    //        if (data.type == angleType)
    //            return data;
    //    }
    //    throw new System.Exception($"No angle data with type {angleType}");
    //}
    #endregion

    #region Scale
    private IEnumerator ScaleRoutine()
    {
        float timeElapsed = 0.0f;
        float progress = 0.0f;

        AnimatorClipInfo[] clipInfos = _animator.GetCurrentAnimatorClipInfo(0);
        AnimationClip currentClip = clipInfos[0].clip;
        float animationLength = currentClip.length;

        while (timeElapsed <= animationLength)
        {
            progress = _scaleCurve.Evaluate(timeElapsed / animationLength);
            float newScale = Mathf.Lerp(_minMaxScale.x, _minMaxScale.y, progress);
            _rect.localScale = new Vector2(newScale, newScale);
            _rect.anchoredPosition = _positionAt1Scale * newScale;

            yield return null;
            timeElapsed += Time.deltaTime;
        }
    }
    #endregion

    #region Animation events
    public void LaunchCheckPlayerCoatInAnim()
    {
        _onCheckPlayerCoat?.Invoke();
        PopUpManager.Instance.LaunchCheckPlayerCoat();

        _frameImage.sprite = _warningFrameImage;
        _warningFrameCoroutine = StartCoroutine(WarningFrame());
    }
    private IEnumerator WarningFrame()
    {
        yield return new WaitForSeconds(_warningImageTime);
        _frameImage.sprite = _defaultFrameImage;
    }

    public void LaunchPolicePatrol()
    {
        PopUpManager.Instance.LaunchPolicePatrol();
    }

    public void LaunchEndAnim()
    {
        Destroy(gameObject);
    }
    #endregion

    #region Game over
    private void StopAnimation()
    {
        _animator.speed = 0;

        //Stop coroutine
        if(_scaleFrameRoutine != null)
        {
            StopCoroutine(_scaleFrameRoutine);
            _scaleFrameRoutine = null;
        }

        //Stop coroutine
        if(_warningFrameCoroutine != null)
        {
            StopCoroutine(_warningFrameCoroutine);
            _warningFrameCoroutine = null;
        }
    }
    #endregion
}
