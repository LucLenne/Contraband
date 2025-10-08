using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;

public class RythmManager : MonoBehaviour
{
    private enum ChangeType
    {
        EachXSeconds,
        EachXClient,
    }

    private enum ValueToIncrease
    {
        ReduceClientPatience,
        ReducePopUpDelay,
        IncreasePopUpSpeed
    }

    [System.Serializable]
    private class RythmRule
    {
        public ChangeType TypeOfChange;
        [Tooltip("Nombre de X secondes ou de X clients")]
        public float NumberOfX;
        [Space(10)]
        public ValueToIncrease ValueToIncrease;
        [Tooltip("De combien augmenter / réduire la valeur")]
        public float ValueAmount;

        [HideInInspector] public bool WasRuleApplied;
    }

    public static RythmManager Instance;

    [Header("List")]
    [SerializeField] private List<RythmRule> _rythmRules;

    [Header("Base Values")]
    [SerializeField] private float _baseClientPatience;
    [SerializeField, MinMaxSlider(1f, 20f)] private Vector2 _basePopUpMinMaxDelay;
    [SerializeField] private float _basePopUpSpeed;

    private float _timeElasped;

    //Current values
    [Space(10)]
    [ReadOnly] public float _currentClientPatience;
    [ReadOnly] public Vector2 _currentPopUpMinMaxDelay;
    [ReadOnly] public float _currentPopUpSpeed;


    public float ClientPatience { get => _currentClientPatience; }
    public Vector2 PopUpMinMaxDelay { get => _currentPopUpMinMaxDelay; }
    public float PopUpSpeed { get => _currentPopUpSpeed; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        _timeElasped = 0.0f;

        _currentClientPatience = _baseClientPatience;
        _currentPopUpMinMaxDelay = _basePopUpMinMaxDelay;
        _currentPopUpSpeed = _basePopUpSpeed;
    }

    private void Update()
    {
        _timeElasped += Time.deltaTime;

        //Check every rule
        foreach (RythmRule rule in _rythmRules)
        {
            switch (rule.TypeOfChange)
            {
                case ChangeType.EachXSeconds:
                    if (rule.NumberOfX <= 0)
                        break;

                    float modulo = _timeElasped % rule.NumberOfX;
                    if (modulo < 0.01f || rule.NumberOfX - modulo < 0.01f)
                    {
                        rule.WasRuleApplied = false;
                        break;
                    }
                    if (rule.WasRuleApplied)
                        break;

                    ChangeValue(rule.ValueToIncrease, rule.ValueAmount);
                    rule.WasRuleApplied = true;
                    break;

                case ChangeType.EachXClient:
                    //Check if isn't at each client
                    if(rule.NumberOfX < 2)
                    {
                        Debug.LogError("Number of client can't be under 2 - go complaint to GP");
                        break;
                    }    

                    if ((int)(LevelManager.Instance.NumberOfClientsEncountered % (int)rule.NumberOfX) != 0)
                    {
                        rule.WasRuleApplied = false;
                        break;
                    }
                    if (rule.WasRuleApplied)
                        break;

                    ChangeValue(rule.ValueToIncrease, rule.ValueAmount);
                    rule.WasRuleApplied = true;
                    break;
            }
        }
    }

    private void ChangeValue(ValueToIncrease valueType, float value)
    {
        switch (valueType)
        {
            case ValueToIncrease.ReduceClientPatience:
                _currentClientPatience -= value;
                _currentClientPatience = Mathf.Max(0, _currentClientPatience);
                return;

            case ValueToIncrease.ReducePopUpDelay:
                _currentPopUpMinMaxDelay.x -= value;
                _currentPopUpMinMaxDelay.y -= value;
                _currentPopUpMinMaxDelay.x = Mathf.Max(0, _currentPopUpMinMaxDelay.x);
                _currentPopUpMinMaxDelay.y = Mathf.Max(0, _currentPopUpMinMaxDelay.y);
                return;

            case ValueToIncrease.IncreasePopUpSpeed:
                _currentPopUpSpeed += value;
                _currentPopUpSpeed = Mathf.Max(0, _currentPopUpSpeed);
                return;
        }
    }
}
