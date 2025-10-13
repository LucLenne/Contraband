using UnityEngine;

public class GameOver : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameOverScene _gameOverScene;
    [SerializeField] private GameObject _gameOverUI;
    [SerializeField] private GameOverHighscore _gameOverHighscore;
    [Space(5)]
    [SerializeField] private int _timeOnSceneGameOver = 3;
    private const string _nameSceneMainMenu = "MainMenu";

    private bool _isWaitingForSceneRestart;

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

        //Check if should restart on highscore or scene
        if (_gameOverHighscore.CheckHighscore())
        {
            _gameOverHighscore.OnRestartGame += RestartGame;
            _isWaitingForSceneRestart = false;
        }
        else
        {
            _gameOverScene.OnGameCardSpawnEnded += RestartGame;
            _isWaitingForSceneRestart = true;
        }
    }

    private void RestartGame()
    {
        //Unbind correct event
        if(_isWaitingForSceneRestart)
            _gameOverScene.OnGameCardSpawnEnded -= RestartGame;
        else
            _gameOverHighscore.OnRestartGame -= RestartGame;

        LoadingManager.Instance.LoadScene(_nameSceneMainMenu);
    }
}
