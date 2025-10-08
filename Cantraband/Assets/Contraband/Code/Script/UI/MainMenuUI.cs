using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private string _tagStartCard;
    private const string levelGameName = "Game";
    private void OnEnable()
    {
        InputManager.Instance.OnReadCard += CheckChard;
    }

    private void OnDisable()
    {
        InputManager.Instance.OnReadCard -= CheckChard;
    }

    void CheckChard(string tag)
    {
        if (tag == _tagStartCard)
        {
            StartGame();
        }
    }

    private void StartGame()
    {
        LoadingManager.Instance.LoadScene(levelGameName);
    }
}
