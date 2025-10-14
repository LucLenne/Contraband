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
    private enum TypePopup
    {
        FakePatrol,
        FakeCamera,
        RealCamera,
        RealPatrol
    }

    private const string CAR_STOP_SOUND = "FOL_car_Stop";
    private const string CAMERA_LOOKING_PLAYER_SOUND = "SFX_Camera_spoting";

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
    [SerializeField] private List<AngleData> _angleDatas;
    [SerializeField] private TypePopup _typePopUp;

    [Header("Render Texture")]
    [SerializeField] private RawImage _rawImage;
    [SerializeField] private Camera _cameraRender;
    [SerializeField] private Vector3Int _textureResolution = new Vector3Int(512,512,16);

    [Header("Animations")]
    [SerializeField] private string _camDeathTriggerName;

    [Header("Parameters")]
    [SerializeField] private float _speed = 1f;
    [SerializeField] private bool _playCameraNoise = false;

    [Header("Warning Image")]
    [SerializeField] private float _warningImageTime;

    [Header("Scale")]
    [SerializeField, MinMaxSlider(0, 10)] private Vector2 _minMaxScale;
    [SerializeField] private AnimationCurve _scaleCurve;

    [Header("Events")]
    [SerializeField] private UnityEvent _onCheckPlayerCoat;

    private RenderTexture _camRenderTexture;

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

        //Setup render texture
        _camRenderTexture = new RenderTexture(_textureResolution.x, _textureResolution.y, _textureResolution.z);
        _cameraRender.targetTexture = _camRenderTexture;
        _rawImage.texture = _camRenderTexture;

        _scaleFrameRoutine = StartCoroutine(ScaleRoutine());
    }

    private void OnEnable()
    {
        if (LevelManager.Instance != null) 
        {
            LevelManager.Instance.OnGameOver += StopAnimation;

        }

    }

    private void OnDisable()
    {
        if(LevelManager.Instance != null)
        {
            LevelManager.Instance.OnGameOver -= StopAnimation;

        }
    }

    #region setup
    public void SetupPopup(AngleType angle, float speedOverride = -1)
    {
        AngleData angleData = FindDataByAngleType(angle);
        //Setup anchors & positions
        _rect.anchorMin = angleData.anchorMinMax;
        _rect.anchorMax = angleData.anchorMinMax;
        _rect.anchoredPosition = angleData.anchoredPosition;
        _positionAt1Scale = angleData.anchoredPosition;

        //Setup images
        _defaultFrameImage = angleData.defaultImage;
        _frameImage.sprite = _defaultFrameImage;
        _warningFrameImage = angleData.OnWarningImage;

        //Setup speed
        if(speedOverride != -1)
            _speed = speedOverride;
    }
    private AngleData FindDataByAngleType(AngleType angleType)
    {
        foreach (AngleData data in _angleDatas)
        {
            if (data.type == angleType)
                return data;
        }
        throw new System.Exception($"No angle data with type {angleType}");
    }
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
            _rect.localScale = new Vector3(newScale, newScale, newScale);
            _rect.anchoredPosition = _positionAt1Scale * newScale;

            yield return null;
            timeElapsed += Time.deltaTime;
        }
    }
    #endregion

    #region Animation events
    public void LaunchCheckPlayerCoatInAnim()
    {
        if (_typePopUp != TypePopup.RealCamera)
            return;

        if (_playCameraNoise)
            AudioManager.AudioManager.Instance.PlaySound(CAMERA_LOOKING_PLAYER_SOUND);

        _frameImage.sprite = _warningFrameImage;
        _warningFrameCoroutine = StartCoroutine(WarningFrame());

        _onCheckPlayerCoat?.Invoke();
        
        if (PopUpManager.Instance.LaunchCheckPlayerCoat())
        {
            LaunchCamDeath();
        }
    }
    private IEnumerator WarningFrame()
    {
        yield return new WaitForSeconds(_warningImageTime);
        _frameImage.sprite = _defaultFrameImage;
    }

    public void LaunchPolicePatrol()
    {
        if (_typePopUp != TypePopup.RealPatrol)
            return;

        AudioManager.AudioManager.Instance.PlaySound(CAR_STOP_SOUND);
        PopUpManager.Instance.LaunchPolicePatrol();
        _frameImage.sprite = _warningFrameImage;
        _warningFrameCoroutine = StartCoroutine(WarningFrame());
    }

    public void LaunchTrigger(string triggerName) 
    { 
        _animator.SetTrigger(triggerName); 
    }

    public void LaunchTriggerPolicePatrol(string triggerName)
    {
        if (_typePopUp != TypePopup.RealPatrol)
            return;

        _animator.SetTrigger(triggerName);
    }

    public void LaunchTriggerFakeCamera(string triggerName)
    {
        if (_typePopUp != TypePopup.FakeCamera)
            return;

        _animator.SetTrigger(triggerName);
    }

    public void LaunchEndAnim()
    {
        Destroy(gameObject);
    }


    private void LaunchCamDeath()
    {
        if(_typePopUp != TypePopup.RealCamera)
            return;

        _animator.SetTrigger(_camDeathTriggerName);
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
