using TMPro;
using UnityEngine;

public class DisplayScore : MonoBehaviour
{
    [SerializeField]private TMP_Text _scoreText;



    private void OnEnable()
    {
        LevelManager.Instance.onNextClientAction += UpdateScore;
    }

    private void OnDisable()
    {
        LevelManager.Instance.onNextClientAction -= UpdateScore;
    }

    void UpdateScore(Client client)
    {
        _scoreText.text = LevelManager.Instance.score.ToString();
    }
}
