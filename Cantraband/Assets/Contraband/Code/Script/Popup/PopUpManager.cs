using System;
using System.Collections.Generic;
using UnityEngine;
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
    [SerializeField] private List<PopUpGeneric> _policePopUps;
    [SerializeField] private List<PopUpGeneric> _feedBackPopUps;

    private Dictionary<AngleType, PopUpGeneric> _occupiedAngles;

    public Action CheckPlayerCoat;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void SpawnRandomPolicePopup()
    {
        //Select random police popup
        PopUpGeneric popUpToSpawn = _policePopUps[Random.Range(0, _policePopUps.Count)];
        //Select random unoccupied angle
        AngleType angleType = SelectUnoccupiedAngle();
        if (angleType == AngleType.None)
            return;

        SpawnPopup(popUpToSpawn, angleType);
    }

    public void SpawnRandomFeedbackPopup()
    {
        //Select random police popup
        PopUpGeneric popUpToSpawn = _feedBackPopUps[Random.Range(0, _feedBackPopUps.Count)];
        //Select random unoccupied angle
        AngleType angleType = SelectUnoccupiedAngle();
        if (angleType == AngleType.None)
            return;

        SpawnPopup(popUpToSpawn, angleType);
    }

    private void SpawnPopup(PopUpGeneric popUpToSpawn, AngleType angle)
    {
        PopUpGeneric newPopUp = Instantiate(popUpToSpawn);
        newPopUp.SetupPopup(angle);
    }

    private AngleType SelectUnoccupiedAngle()
    {
        int angleInt = Random.Range(1, 5);
        for (int i = 0; i < 4; i++)
        {
            if (_occupiedAngles[(AngleType)angleInt] == null)
            {
                return (AngleType)angleInt;    
            }

            angleInt = (angleInt + 1 == 5) ? 1 : angleInt++;
        }
        Debug.LogWarning("No angle found");
        return (AngleType)angleInt;
    }
}
