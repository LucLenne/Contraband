using TMPro;
using UnityEngine;

public class DisplayScore : MonoBehaviour
{
    [SerializeField]private TMP_Text _scoreText;



    private void OnEnable()
    {
        LevelManager.Instance.OnValidateTransaction += UpdateScore;
    }

    private void OnDisable()
    {
        LevelManager.Instance.OnValidateTransaction -= UpdateScore;
    }

    void UpdateScore()
    {
        _scoreText.text = LevelManager.Instance.score.ToString();
    }
}
