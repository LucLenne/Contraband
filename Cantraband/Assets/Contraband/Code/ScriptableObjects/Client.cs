using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Client", menuName = "Scriptable Objects/Client")]
public class Client : ScriptableObject
{
    public Sprite clue;
    public string text;
    public bool isPolice;
    public List<GameGenre> themePreference;
    public GameCard favoriteCard;
}
