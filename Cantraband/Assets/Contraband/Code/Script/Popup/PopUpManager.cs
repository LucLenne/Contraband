using NaughtyAttributes;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

public class PopUpManager : MonoBehaviour
{
    public enum AngleType
    {
        None,
        UpperLeft,
        UpperRight,
        LowerLeft,
        LowerRight,
    }

    public static PopUpManager Instance;

    [Header("References")]
    [SerializeField] private Transform _parentCanvas;
    [Space(5)]
    [SerializeField] private List<PopUpGeneric> _policePopUps;
    [SerializeField] private List<PopUpGeneric> _feedBackPopUps;
    [Space(5)]
    [SerializeField] private List<PopUpGeneric> _patrolPolicePopUpReferenceList;

    [Header("Events")]
    [SerializeField] private UnityEvent _onCheckPlayerCoat;
    [SerializeField] private UnityEvent _onLaunchPolicePatrol;


    private Dictionary<AngleType, PopUpGeneric> _occupiedAngles = new Dictionary<AngleType, PopUpGeneric>();

    public Action<AngleType> OnNewPopup;
    public Action<AngleType, float> OnUpdateProgress;
    public Action<AngleType> OnLeavePopup;

    public Action OnCheckPlayerCoat;
    public Action OnLaunchPolicePatrol;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        //Init dictionary
        _occupiedAngles.Add(AngleType.UpperLeft, null);
        _occupiedAngles.Add(AngleType.UpperRight, null);
        _occupiedAngles.Add(AngleType.LowerLeft, null);
    }

    #region Spawn popup
    [Button]
    public void SpawnRandomPolicePopup()
    {
        if (_policePopUps.Count <= 0)
            throw new System.Exception("Police list is empty");

        //Select random police popup
        PopUpGeneric popUpToSpawn = _policePopUps[Random.Range(0, _policePopUps.Count)];
        if (PolicePatrolPopUpExists() && _patrolPolicePopUpReferenceList.Contains(popUpToSpawn))
            return;

        //Select random unoccupied angle
        AngleType angleType = SelectUnoccupiedAngle();
        if (angleType == AngleType.None)
            return;
        float popUpSpeed;
        if (RythmManager.Instance == null)
        {
            popUpSpeed = 1;
        }
        else
        {
            popUpSpeed = RythmManager.Instance.PopUpSpeed;
        }

        OnNewPopup?.Invoke(angleType);
        SpawnPopup(popUpToSpawn, angleType, popUpSpeed);
    }

    [Button]
    public void SpawnRandomFeedbackPopup()
    {
        if (_feedBackPopUps.Count <= 0)
            throw new System.Exception("Feed back list is empty");

        //Select random police popup
        PopUpGeneric popUpToSpawn = _feedBackPopUps[Random.Range(0, _feedBackPopUps.Count)];
        //Select random unoccupied angle
        AngleType angleType = SelectUnoccupiedAngle();
        if (angleType == AngleType.None)
            return;

        SpawnPopup(popUpToSpawn, angleType);
    }

    private void SpawnPopup(PopUpGeneric popUpToSpawn, AngleType angle, float speed = -1f)
    {
        PopUpGeneric newPopUp = Instantiate(popUpToSpawn, _parentCanvas);
        _occupiedAngles[angle] = newPopUp;
        newPopUp.SetupPopup(angle, speed);

    }

    private AngleType SelectUnoccupiedAngle()
    {
        int angleInt = Random.Range(1, 5);
        for (int i = 0; i < 4; i++)
        {
            if (_occupiedAngles.ContainsKey((AngleType)angleInt))
            {
                if (_occupiedAngles[(AngleType)angleInt] == null)
                {
                    return (AngleType)angleInt;
                }
            }

            angleInt++;
            if (angleInt == 5)
                angleInt = 1;
        }

        Debug.LogWarning("No angle found");
        return AngleType.None;
    }

    private bool PolicePatrolPopUpExists()
    {
        for(int i = 1; i < 5; i++)
        {
            if (_occupiedAngles.ContainsKey((AngleType)i))
            {
                if (_patrolPolicePopUpReferenceList.Contains(_occupiedAngles[(AngleType)i]))
                    return true;
            }

        }
        return false;
    }
    #endregion

    #region Launch events
    public bool LaunchCheckPlayerCoat()
    {
        _onCheckPlayerCoat?.Invoke();
        OnCheckPlayerCoat?.Invoke();

        if(LevelManager.Instance != null)
        {
            return LevelManager.Instance.CheckPlayerCoat();
        }
        return false;
    }

    public void LaunchPolicePatrol()
    {
        _onLaunchPolicePatrol?.Invoke();
        OnLaunchPolicePatrol?.Invoke();
    }

    #endregion
}
