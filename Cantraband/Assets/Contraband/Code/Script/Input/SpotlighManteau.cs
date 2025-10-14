using UnityEngine;

public class SpotlighManteau : MonoBehaviour
{
    [SerializeField] private Light _light;

    private void OnEnable()
    {
        InputManager.Instance.OnVestChanged += ChangeSpotlight;
    }

    private void OnDisable()
    {
        InputManager.Instance.OnVestChanged -= ChangeSpotlight;
    }

    private void ChangeSpotlight(bool opened)
    {
        _light.enabled = opened;
    }
}
