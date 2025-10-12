using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameOverScene _gameOverScene;
    [SerializeField] private GameObject _gameOverUI;
    [SerializeField] private GameOverHighscore _gameOverHighscore;
    [Space(5)]
    [SerializeField] private int _timeOnSceneGameOver = 3;
    private const string _nameSceneMainMenu = "MainMenu";

    private void Awake()
    {
        _gameOverUI.SetActive(false);
        _gameOverScene.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        LevelManager.Instance.OnGameOver += ActivateGameOver;
    }

    private void OnDisable()
    {
        LevelManager.Instance.OnGameOver -= ActivateGameOver;
    }

    private void ActivateGameOver()
    {
        _gameOverUI.SetActive(true);
        _gameOverScene.gameObject.SetActive(true);
        _gameOverScene.OnGameCardSpawnEnded += RestartGame;
        _gameOverHighscore.CheckHighscore();
    }

    private void RestartGame()
    {
        _gameOverScene.OnGameCardSpawnEnded -= RestartGame;
        LoadingManager.Instance.LoadScene(_nameSceneMainMenu);
    }
}
