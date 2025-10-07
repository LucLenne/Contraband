using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{
    [SerializeField] private int _timeOnSceneGameOver = 3;
    private const string _nameSceneMainMenu = "Game";
    [SerializeField] private GameObject _gameOverUI;

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
        _gameOverUI.SetActive(true);
        await Tasks.WaitSeconds(_timeOnSceneGameOver);
        RestartGame();
    }

    private void RestartGame()
    {
        SceneManager.LoadScene(_nameSceneMainMenu);
    }
}
