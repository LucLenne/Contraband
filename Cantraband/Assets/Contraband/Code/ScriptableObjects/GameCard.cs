using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameCard", menuName = "Scriptable Objects/GameCard")]
public class GameCard : ScriptableObject
{
    public int Tag;
    public string Name;
    public List<GameGenre> Genres;
    public List<Sprite> HintImages;
}
