using System;
using System.Collections.Generic;
using UnityEngine;

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


    public Action CheckPlayerCoat;

    private void Awake()
    {
        if(Instance != null &&  Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
}
