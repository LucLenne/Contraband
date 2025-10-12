using System.Collections.Generic;
using UnityEngine;

public class HighscoreManager : MonoBehaviour
{
    public static HighscoreManager Instance;

    [System.Serializable]
    public struct HighscoreDataStruct
    {
        public string Name;
        public int Score;
    }

    [Header("Parameters")]
    [SerializeField] private int _maxNumberOfHighScore;

    [SerializeField]
    private List<HighscoreDataStruct> _highscoreData = new List<HighscoreDataStruct>();

    public List<HighscoreDataStruct> HighscoreData { get => _highscoreData; }

    private void OnValidate()
    {
        _maxNumberOfHighScore = Mathf.Max(_maxNumberOfHighScore, 0);
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public bool CheckNewHighScore(int newScore)
    {
        if (HighscoreData.Count >= _maxNumberOfHighScore)
            return _highscoreData[_highscoreData.Count - 1].Score <= newScore;
        else
            return true;
    }

    public void AddHighScore(string name, int score)
    {
        //Check where to place it 
        int index = 0;
        if(HighscoreData.Count != 0)
        {
            while (HighscoreData[index].Score >= score && index < HighscoreData.Count)
                index++;
        }

        //Add highscore
        HighscoreData.Insert(index, new HighscoreDataStruct { Name = name, Score = score });

        //Remove last highscore if overflow
        if(_maxNumberOfHighScore < _highscoreData.Count)
            HighscoreData.RemoveAt(_highscoreData.Count - 1);
    }
}
