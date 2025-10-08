using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Client", menuName = "Scriptable Objects/Client")]
public class Client : ScriptableObject
{
    public GameObject clientPrefab;
    public bool isPolice;
    public List<GameGenre> genrePreference;
    public GameCard favoriteCard;
}
