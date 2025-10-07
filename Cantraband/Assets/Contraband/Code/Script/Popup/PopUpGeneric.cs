using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
    [SerializeField] private List<AngleData> _angleDatas;

    [Header("Parameters")]
    [SerializeField] private float _speed = 1f;

    [Header("Warning Image")]
    [SerializeField] private float _warningImageTime;

    private Sprite _defaultFrameImage;
    private Sprite _warningFrameImage;

    private Coroutine _warningFrameCoroutine;

    public float Speed 
    { 
        get => _speed; 
        set => _speed = value; 
    }

    private void Awake()
    {
        _animator.speed = _speed;
    }

    public void SetupPopup(AngleType angle)
    {
        AngleData angleData = FindDataByAngleType(angle);
        _rect.anchorMin = angleData.anchorMinMax;
        _rect.anchorMax = angleData.anchorMinMax;
        _rect.anchoredPosition = angleData.anchoredPosition;

        //Setup images
        _defaultFrameImage = angleData.defaultImage;
        _frameImage.sprite = _defaultFrameImage;
        _warningFrameImage = angleData.OnWarningImage;
    }
    private AngleData FindDataByAngleType(AngleType angleType)
    {
        foreach(AngleData data in _angleDatas)
        {
            if(data.type == angleType)
                return data;
        }
        throw new System.Exception($"No angle data with type {angleType}");
    }


    public void LaunchEventInAnim()
    {
        PopUpManager.Instance.CheckPlayerCoat?.Invoke();
        
        _frameImage.sprite = _warningFrameImage;
        _warningFrameCoroutine = StartCoroutine(WarningFrame());
    }
    private IEnumerator WarningFrame()
    {
        yield return new WaitForSeconds(_warningImageTime);
        _frameImage.sprite = _defaultFrameImage;
    }

    public void LaunchEndAnim()
    {
        Destroy(gameObject);
    }
}
