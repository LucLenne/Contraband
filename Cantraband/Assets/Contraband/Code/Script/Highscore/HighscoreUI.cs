using TMPro;
using UnityEngine;
using static HighscoreManager;

public class HighscoreUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform _parentVerticalGroup;
    [SerializeField] private TMP_Text _highscoreTextPrefab;

    [Header("Text")]
    [Tooltip("{0} = number, {1} = nom, {2} = score")]
    [SerializeField] private string _firstHighscoreTextFormat;
    [Tooltip("{0} = number, {1} = nom, {2} = score")]
    [SerializeField] private string _highscoreTextFormat;

    private void Start()
    {
        for (int i = 0; i < HighscoreManager.Instance.HighscoreData.Count; i++)
        {
            TMP_Text newScoreText = Instantiate(_highscoreTextPrefab, _parentVerticalGroup);
            string formattedText;
            if (i == 0)
            {
                formattedText = string.Format(
                    _firstHighscoreTextFormat,
                    i + 1,
                    HighscoreManager.Instance.HighscoreData[i].Name,
                    HighscoreManager.Instance.HighscoreData[i].Score
                );
            }
            else
            {
                formattedText = string.Format(
                    _highscoreTextFormat,
                    i + 1,
                    HighscoreManager.Instance.HighscoreData[i].Name,
                    HighscoreManager.Instance.HighscoreData[i].Score
                );
            }

            newScoreText.text = formattedText;
        }
    }
}
