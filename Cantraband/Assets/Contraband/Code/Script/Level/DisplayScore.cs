using TMPro;
using UnityEngine;

public class DisplayScore : MonoBehaviour
{
    [SerializeField]private TMP_Text _scoreText;



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
        _scoreText.text = LevelManager.Instance.score.ToString();
    }
}
