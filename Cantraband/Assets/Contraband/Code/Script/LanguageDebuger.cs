using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

public class LanguageDebuger : MonoBehaviour
{
    [SerializeField] Locale _defaultLocale;
    [SerializeField] Locale _englishLocale;
    [SerializeField] Locale _frenchLocale;

    private void Awake()
    {
        LocalizationSettings.SelectedLocale = _defaultLocale;
    }

    void Update()
    {
        if(Keyboard.current[Key.F1].wasPressedThisFrame)
            LocalizationSettings.SelectedLocale = _englishLocale;
        else if (Keyboard.current[Key.F2].wasPressedThisFrame)
            LocalizationSettings.SelectedLocale = _frenchLocale;
    }
}
