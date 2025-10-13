using UnityEngine;

public class MainMenuUI : MonoBehaviour
{
    private const string MAIN_MENU_START_SOUND = "SFX_Console_Start";

    [Header("Tutorial")]
    [SerializeField] private string _tagTutoStartCard;
    [SerializeField] private string _tagTutoClavierDebug;

    [Header("Game")]
    [SerializeField] private string _tagGameStartCard;
    [SerializeField] private string _tagGameClavierDebug;

    private const string NAME_TUTO_LEVEL = "Tuto";
    private const string NAME_GAME_LEVEL = "Game";

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
        if (tag == _tagTutoStartCard || tag == _tagTutoClavierDebug)
            StartTuto();
        else if (tag == _tagGameStartCard || tag == _tagGameClavierDebug)
            StartGame();
    }

    private void StartTuto() => LoadingManager.Instance.LoadScene(NAME_TUTO_LEVEL, true);
    private void StartGame() => LoadingManager.Instance.LoadScene(NAME_GAME_LEVEL, true);
}
