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

        public bool WasRuleApplied;
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
                    if ((int)(_timeElasped % rule.NumberOfX) != 0)
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
                    if ((int)(LevelManager.Instance.NumberOfClientsEncountered % rule.NumberOfX) != 0)
                    {
                        rule.WasRuleApplied = false;
                        break;
                    }
                                            //Check if isn't at each client
                    if (rule.WasRuleApplied && rule.NumberOfX != 1)
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
                return;

            case ValueToIncrease.ReducePopUpDelay:
                _currentPopUpMinMaxDelay.x -= value;
                _currentPopUpMinMaxDelay.y -= value;
                return;

            case ValueToIncrease.IncreasePopUpSpeed:
                _basePopUpSpeed += value;
                return;
        }
    }
}
