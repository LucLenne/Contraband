using System;
using TMPro;
using UnityEngine;

public class DisplayTimer : MonoBehaviour
{
    [Header("Text")]
    [SerializeField] private TMP_Text _timerText;
    [SerializeField] private string _textAnims = "<+spread><wave><palette><-fade>";

    [Header("Anim")]
    [SerializeField] private float _amplitude;
    [SerializeField] private float _speed;
    [SerializeField] private AnimationCurve _amplitudeCurve;
    [SerializeField] private AnimationCurve _speedCurve;

    float _defaultFontSize;
    private void Awake()
    {
        _defaultFontSize = _timerText.fontSize;
    }

    private void OnEnable()
    {
        LevelManager.Instance.OnDebugStopTimer += DebugStopTimer;
    }

    private void OnDisable()
    {
        LevelManager.Instance.OnDebugStopTimer -= DebugStopTimer;
    }

    private void Start()
    {
        //Desactive le texte si le timer est inactive
        if (!LevelManager.Instance.IsGameTimerActive)
            _timerText.text = "";
    }

    //Methode de debug au cas ou les GD veulent pas du timer finalement
    private void DebugStopTimer() => _timerText.text = "";

    void Update()
    {
        if (LevelManager.Instance != null && LevelManager.Instance.IsGameTimerActive)
        {
            _timerText.text = _textAnims + ((int)LevelManager.Instance.RemainingTime).ToString();
            float progress = (LevelManager.Instance.GameTime - LevelManager.Instance.RemainingTime) / LevelManager.Instance.GameTime;
            _timerText.fontSize = _defaultFontSize + (Mathf.Sin(Time.time * _speedCurve.Evaluate(progress) * _speed) * _amplitudeCurve.Evaluate(progress) * _amplitude);
        }
    }
}
