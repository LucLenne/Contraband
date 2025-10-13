using System.Collections;
using UnityEngine;
using UnityEngine.UI;


[RequireComponent(typeof(Image))]
public class SlowRevealImage : MonoBehaviour
{

    // Fields
    [SerializeField] Image BaseImage;
    [SerializeField] RevealType _revealType;
    [Min(1.0f)]
    [SerializeField] public float baseDurationReveal;
    public bool revealOnAwake = false;
    [SerializeField] private AnimationCurve fadeCurve = AnimationCurve.Linear(0, 0, 1, 1);

    //Events
    public System.Action<float, RevealType> CallForReveal;

    // Vars
    private Color savedColor;

    private void Awake()
    {
        if (BaseImage == null)
        {
            Destroy(this);
        }
        CallForReveal += StartReveal;
        savedColor = BaseImage.color;
        BaseImage.color = new Color(savedColor.r, savedColor.g, savedColor.b, 0);
    }


    private void OnDisable()
    {
        CallForReveal -= StartReveal;
    }
    public void StartReveal(float inDuration = -1, RevealType inRevealType = default)
    {
        if (inDuration == -1)
            inDuration = baseDurationReveal;

        StopAllCoroutines(); // Stop any existing fade

        switch (_revealType)
        {
            case RevealType.Fade:
                StartCoroutine(SmoothFadeReveal(inDuration));
                break;
            default:
                BaseImage.color = new Color(1, 1, 1, 1);
                break;
        }
    }

    private IEnumerator SmoothFadeReveal(float inDuration)
    {
        float elapsed = 0.0f;

        while (elapsed < inDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Clamp01(elapsed / inDuration);
            float curvedAlpha = fadeCurve.Evaluate(alpha);
            BaseImage.color = new Color(savedColor.r, savedColor.g, savedColor.b, curvedAlpha);
            yield return null;
        }
        BaseImage.color = new Color(savedColor.r, savedColor.g, savedColor.b, 1);
    }
}
public enum RevealType
{
    Fade,
    None,
}
