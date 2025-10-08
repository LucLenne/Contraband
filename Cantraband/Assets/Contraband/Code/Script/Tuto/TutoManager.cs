using System.Collections.Generic;
using UnityEngine;

public class TutoManager : MonoBehaviour
{
    [SerializeField] private Baron _baron;

    [Header("3 Clients"), SerializeField] private List<Client> _listClient;

    private void OnEnable()
    {
        InputManager.Instance.OnReadCard += CheckCard;
    }

    private void OnDisable()
    {
        InputManager.Instance.OnReadCard -= CheckCard;
    }

    void CheckCard(int id)
    {
        
    }


}
