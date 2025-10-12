using UnityEngine;

public class GameOverHighscore : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private RectTransform _nameInputUI;

    public void CheckHighscore()
    {
        int currentScore = LevelManager.Instance.score;
        if (HighscoreManager.Instance.CheckNewHighScore(currentScore))
        {
            throw new System.NotImplementedException();
        }
    }

    public void AddNewHighscore(string name)
    {
        throw new System.NotImplementedException();
    }
}
