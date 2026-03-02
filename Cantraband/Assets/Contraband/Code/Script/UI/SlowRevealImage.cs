using System.Collections;
using UnityEngine;
using UnityEngine.UI;


[RequireComponent(typeof(Image))]
public class SlowRevealImage : MonoBehaviour
{

    // Fields
    [SerializeField] Image BaseImage;
    [SerializeField] Image _gameImage;
    [SerializeField] RevealType _revealType;
    [Min(1.0f)]
    [SerializeField] public float baseDurationReveal;
    public bool revealOnAwake = false;
    [SerializeField] private AnimationCurve fadeCurve = AnimationCurve.Linear(0, 0, 1, 1);

    [Header("FadeInOut")]
    [SerializeField] private float _fadeInOutSpeed;
    [Tooltip("Idéalement laisser a 1, car va osciller entre 0 et l'amplitude")]
    [SerializeField] private float _fadeInOutAmplitude;
    [Tooltip("De combien ajouter le sinus (sachant qu'à 0, la moitié du sin est en dessous de 0 donc on le voit pas 50% du temps) (si c'est pas clair pingez moi ou utiliser GeoGebra pour visualiser)")]
    [SerializeField] private float _fadeInOutOffset;

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
        _gameImage.color = new Color(savedColor.r, savedColor.g, savedColor.b, 0);
    }


    private void OnDisable()
    {
        CallForReveal -= StartReveal;
    }

    public void Setup(Sprite genreImage, Sprite gameImage)
    {
        BaseImage.sprite = genreImage;
        _gameImage.sprite = gameImage;
    }

    public void StartReveal(float inDuration = -1, RevealType inRevealType = default)
    {
        if (inDuration == -1)
            inDuration = baseDurationReveal;

        StopAllCoroutines(); // Stop any existing fade

        switch (inRevealType)
        {
            case RevealType.Fade:
                StartCoroutine(SmoothFadeReveal(inDuration));
                break;
            case RevealType.FadeInOutSin:
                StartCoroutine(FadeInOutReveal());
                break;
            default:
                BaseImage.color = new Color(1, 1, 1, 1);
                _gameImage.color = new Color(1, 1, 1, 1);
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
            _gameImage.color = new Color(savedColor.r, savedColor.g, savedColor.b, curvedAlpha);
            yield return null;
        }
        BaseImage.color = new Color(savedColor.r, savedColor.g, savedColor.b, 1);
        _gameImage.color = new Color(savedColor.r, savedColor.g, savedColor.b, 1);
    }

    private IEnumerator FadeInOutReveal()
    {
        float elapsed = 0.0f;
        while(true)
        {
            elapsed += Time.deltaTime;
            float alpha = (Mathf.Sin(elapsed * _fadeInOutSpeed) * _fadeInOutAmplitude) + _fadeInOutOffset;
            BaseImage.color = new Color(savedColor.r, savedColor.g, savedColor.b, alpha);
            _gameImage.color = new Color(savedColor.r, savedColor.g, savedColor.b, alpha);
            yield return null;
        }
    }
}
public enum RevealType
{
    Fade,
    None,
    FadeInOutSin,
}
