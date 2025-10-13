using LeadMind.Animation2D;
using System;
using UnityEngine;

public class GameOverHighscore : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TransitionAnimation _nameInputUI;

    public Action OnRestartGame;

    public bool CheckHighscore()
    {
        if (HighscoreManager.Instance)
        {
            Debug.LogError("No highscore manager found");
            return false;
        }
            

        int currentScore = LevelManager.Instance.score;
        if (HighscoreManager.Instance.CheckNewHighScore(currentScore))
        {
            _nameInputUI.PlayAnimation();
            return true;
        }
        return false;
    }

    public void AddNewHighscore(string name)
    {
        int currentScore = LevelManager.Instance.score;
        HighscoreManager.Instance.AddHighScore(name, currentScore);
        OnRestartGame?.Invoke();
    }
}
