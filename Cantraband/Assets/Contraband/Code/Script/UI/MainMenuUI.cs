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

    [Header("Creadits")]
    [SerializeField] private string _tagCreditsStartCard;
    [SerializeField] private string _tagCreditsClavierDebug;

    private const string NAME_COAT_TUTO_LEVEL = "Coat Tuto";
    private const string NAME_GAME_LEVEL = "Game";
    private const string NAME_CREDITS_LEVEL = "Credits";

    [SerializeField] AudioSource BGM_Player;

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
        Invoke("StartBGM", 2);
    }

   private void StartBGM()
    {
        if(BGM_Player != null)
        {
            BGM_Player.Play();
        }
    }


    void CheckChard(string tag)
    {
        if (tag == _tagTutoStartCard || tag == _tagTutoClavierDebug)
            StartCoatTuto();
        else if (tag == _tagGameStartCard || tag == _tagGameClavierDebug)
            StartGame();
        else if (tag == _tagCreditsStartCard || tag == _tagCreditsClavierDebug)
            StartCredits();
    }
    private void StartCoatTuto() => LoadingManager.Instance.LoadScene(NAME_COAT_TUTO_LEVEL, true);
    private void StartGame() => LoadingManager.Instance.LoadScene(NAME_GAME_LEVEL, true);
    private void StartCredits() => LoadingManager.Instance.LoadScene(NAME_CREDITS_LEVEL, true);
}
