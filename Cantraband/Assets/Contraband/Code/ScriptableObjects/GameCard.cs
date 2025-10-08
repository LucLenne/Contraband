using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameCard", menuName = "Scriptable Objects/GameCard")]
public class GameCard : ScriptableObject
{
    public string tag;
    public string Name;
    public List<GameGenre> genres;
    public Sprite hintImage;
}
