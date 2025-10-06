using System;
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
        public Vector2 anchorMin;
        public Vector2 anchorMax;
        public Vector2 anchoredPosition;
    }

    [Header("References")]
    [SerializeField] private Animator _animator;
    [SerializeField] private Image _frameImage;
    [SerializeField] private List<AngleData> _angleDatas;

    [Header("Parameters")]
    [SerializeField] private float _speed = 1f;

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
        _frameImage.sprite = angleData.defaultImage;

        //Setup warning Image + anchoredPosition
        throw new NotImplementedException();
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
    }

    public void LaunchEndAnim()
    {
        Destroy(gameObject);
    }
}
