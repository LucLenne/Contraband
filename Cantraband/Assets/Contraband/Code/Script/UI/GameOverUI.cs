using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] private int _timerOnSceneGameover;
    private const string nameSceneMainMenu = "Game";

    private void OnEnable()
    {
        LevelManager.Instance.OnGameOver += OnStayInGameOver;
    }

    private void OnDisable()
    {
        LevelManager.Instance.OnGameOver -= OnStayInGameOver;
    }

    private async void OnStayInGameOver()
    {
        await 
    }

    private void RestartGame()
    {
        SceneManager.LoadScene(nameSceneMainMenu);
    }
}
