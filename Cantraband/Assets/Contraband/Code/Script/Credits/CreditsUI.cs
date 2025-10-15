using UnityEngine;

public class CreditsUI : MonoBehaviour
{
    private const string NAME_MAINMENU_LEVEL = "MainMenu";

    private void ReturnMainMenu() => LoadingManager.Instance.LoadScene(NAME_MAINMENU_LEVEL, false);
}
