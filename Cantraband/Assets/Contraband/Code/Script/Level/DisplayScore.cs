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
        if (LevelManager.Instance != null)
            LevelManager.Instance.OnFinishTransaction += UpdateScore;

        if (TutoManager.Instance != null)
            TutoManager.Instance.onClientLeave += UpdateScore;


    }

    private void OnDisable()
    {
        if (LevelManager.Instance != null)
            LevelManager.Instance.OnFinishTransaction -= UpdateScore;

        if (TutoManager.Instance != null)
            TutoManager.Instance.onClientLeave -= UpdateScore;
    }

    void UpdateScore()
    {
        Debug.Log("Update Score");
        if (LevelManager.Instance != null)
            _scoreText.text = _scoreAnims + LevelManager.Instance.score.ToString();
        if (TutoManager.Instance != null)
            _scoreText.text = _scoreAnims + TutoManager.Instance.score.ToString();

    }
}
