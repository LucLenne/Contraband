using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private int _tagStartCard;
    private const string levelGameName = "Game";
    private void OnEnable()
    {
        InputManager.Instance.OnReadCard += CheckChard;
    }

    private void OnDisable()
    {
        InputManager.Instance.OnReadCard -= CheckChard;
    }

    void CheckChard(int tag)
    {
        if (tag == _tagStartCard)
        {
            StartGame();
        }
    }

    private void StartGame()
    {
        SceneManager.LoadScene(levelGameName);
    }
}
