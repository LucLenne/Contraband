using TMPro;
using UnityEngine;

public class DisplayScore : MonoBehaviour
{
    [SerializeField] private TMP_Text _scoreText;
    [SerializeField] private string _scoreAnims = "<+spread><wave><palette><-fade>";

    private void Start()
    {
        UpdateScore();
    }

    private void OnEnable()
    {
        LevelManager.Instance.OnFinishTransaction += UpdateScore;
    }

    private void OnDisable()
    {
        LevelManager.Instance.OnFinishTransaction -= UpdateScore;
    }

    void UpdateScore()
    {
        _scoreText.text = _scoreAnims + LevelManager.Instance.score.ToString();
    }
}
