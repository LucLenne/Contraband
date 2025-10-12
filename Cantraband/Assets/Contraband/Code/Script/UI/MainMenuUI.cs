using UnityEngine;

public class MainMenuUI : MonoBehaviour
{
    private const string MAIN_MENU_START_SOUND = "SFX_Console_Start";

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

    private void Start()
    {
        AudioManager.AudioManager.Instance.PlaySound(MAIN_MENU_START_SOUND);
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
        LoadingManager.Instance.LoadScene(NAME_NEXT_LEVEL, true);
    }
}
