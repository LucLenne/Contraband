using UnityEngine;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private string _tagStartCard;
    [SerializeField] private string _tagClavierDebug;
    private const string NAME_NEXT_LEVEL = "Tuto";
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
        if (tag == _tagStartCard || tag == _tagClavierDebug)
        {
            StartGame();
        }
    }

    private void StartGame()
    {
        LoadingManager.Instance.LoadScene(NAME_NEXT_LEVEL);
    }
}
