using NaughtyAttributes;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class PopUpManager : MonoBehaviour
{
    //public enum AngleType
    //{
    //    None,
    //    UpperLeft,
    //    UpperRight,
    //    LowerLeft,
    //    LowerRight,
    //}

    public static PopUpManager Instance;

    [Header("References")]
    [SerializeField] private Transform _parentCanvas;
    [Space(5)]
    [SerializeField] private List<PopUpGeneric> _policePopUps;
    [SerializeField] private List<PopUpGeneric> _feedBackPopUps;

    [Header("Events")]
    [SerializeField] private UnityEvent _onCheckPlayerCoat;
    [SerializeField] private UnityEvent _onLaunchPolicePatrol;

    //private Dictionary<AngleType, PopUpGeneric> _occupiedAngles = new Dictionary<AngleType, PopUpGeneric>();

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
        //_occupiedAngles.Add(AngleType.UpperLeft, null);
        //_occupiedAngles.Add(AngleType.UpperRight, null);
        //_occupiedAngles.Add(AngleType.LowerLeft, null);
        //_occupiedAngles.Add(AngleType.LowerRight, null);
    }

    #region Spawn popup
    [Button]
    public void SpawnRandomPolicePopup()
    {
        if (_policePopUps.Count <= 0)
            throw new System.Exception("Police list is empty");

        //Select random police popup
        PopUpGeneric popUpToSpawn = _policePopUps[Random.Range(0, _policePopUps.Count)];
        float popUpSpeed = RythmManager.Instance.PopUpSpeed;
        SpawnPopUp(popUpToSpawn, popUpSpeed);
    }

    [Button]
    public void SpawnRandomFeedbackPopup()
    {
        if (_feedBackPopUps.Count <= 0)
            throw new System.Exception("Feed back list is empty");

        //Select random police popup
        PopUpGeneric popUpToSpawn = _feedBackPopUps[Random.Range(0, _feedBackPopUps.Count)];

        SpawnPopUp(popUpToSpawn);
    }
    private void SpawnPopUp(PopUpGeneric popup, float speed = 1)
    {
        PopUpGeneric newPopUp = Instantiate(popup, _parentCanvas);

    }

    #endregion

    #region Spawn popup OLD SYSTEM
    //[Button]
    //public void SpawnRandomPolicePopup()
    //{
    //    if (_policePopUps.Count <= 0)
    //        throw new System.Exception("Police list is empty");

    //    //Select random police popup
    //    PopUpGeneric popUpToSpawn = _policePopUps[Random.Range(0, _policePopUps.Count)];
    //    //Select random unoccupied angle
    //    AngleType angleType = SelectUnoccupiedAngle();
    //    if (angleType == AngleType.None)
    //        return;

    //    float popUpSpeed = RythmManager.Instance.PopUpSpeed;
    //    SpawnPopup(popUpToSpawn, angleType, popUpSpeed);
    //}

    //[Button]
    //public void SpawnRandomFeedbackPopup()
    //{
    //    if (_feedBackPopUps.Count <= 0)
    //        throw new System.Exception("Feed back list is empty");

    //    //Select random police popup
    //    PopUpGeneric popUpToSpawn = _feedBackPopUps[Random.Range(0, _feedBackPopUps.Count)];
    //    //Select random unoccupied angle
    //    AngleType angleType = SelectUnoccupiedAngle();
    //    if (angleType == AngleType.None)
    //        return;

    //    SpawnPopup(popUpToSpawn, angleType);
    //}

    //private void SpawnPopup(PopUpGeneric popUpToSpawn, AngleType angle, float speed = -1f)
    //{
    //    PopUpGeneric newPopUp = Instantiate(popUpToSpawn, _parentCanvas);
    //    //_occupiedAngles[angle] = newPopUp;
    //    newPopUp.SetupPopup(angle, speed);

    //}

    //private AngleType SelectUnoccupiedAngle()
    //{
    //    int angleInt = Random.Range(1, 5);
    //    for (int i = 0; i < 4; i++)
    //    {
    //        if (_occupiedAngles[(AngleType)angleInt] == null)
    //        {
    //            return (AngleType)angleInt;    
    //        }

    //        angleInt++;
    //        if (angleInt == 5)
    //            angleInt = 1;
    //    }

    //    Debug.LogWarning("No angle found");
    //    return AngleType.None;
    //}
    #endregion

    #region Launch events
    public void LaunchCheckPlayerCoat()
    {
        _onCheckPlayerCoat?.Invoke();
        OnCheckPlayerCoat?.Invoke();
        LevelManager.Instance.CheckPlayerCoat();
    }

    public void LaunchPolicePatrol()
    {
        _onLaunchPolicePatrol?.Invoke();
        OnLaunchPolicePatrol?.Invoke();
    }
    #endregion
}
