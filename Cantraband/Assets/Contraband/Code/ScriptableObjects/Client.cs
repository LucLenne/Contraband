using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Client", menuName = "Scriptable Objects/Client")]
public class Client : ScriptableObject
{
    public Sprite client;
    public Sprite clue;
    public string text;
    public bool isPolice;
    public List<GameGenre> genrePreference;
    public GameCard favoriteCard;
}
