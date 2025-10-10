using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class GameOverScene : MonoBehaviour
{
    private const string SCORE_TEXT = "Score\n";
    private const string SCORE_ANIM = "<+spread><wave><palette><-fade>";

    [Header("References")]
    [SerializeField] private Transform _spawnPoint;
    [SerializeField] private GameObject _musicPlayer;
    [SerializeField] private TMP_Text _scoreText;

    [Header("Delay")]
    [SerializeField] private float _delayBTWspawns;
    [SerializeField] private float _delayBeforeLoadScene;

    private int _currentScore;
    private Coroutine _spawnGameCardsCoroutine;

    public Action OnGameCardSpawnEnded;

    private void OnEnable()
    {
        _spawnGameCardsCoroutine = StartCoroutine(SpawnCardsRoutine());
        _musicPlayer.SetActive(true);
    }

    private IEnumerator SpawnCardsRoutine()
    {
        if (LevelManager.Instance.GameCardsGiven.Count != LevelManager.Instance.PointsAwarded.Count)
            Debug.LogError("number of game cards given isn't the same as poitns awarded");

        for (int i = 0; i < LevelManager.Instance.PointsAwarded.Count; i++)
        {
            //Spawn game cards
            if(i < LevelManager.Instance.GameCardsGiven.Count)
            {
                GameObject prefabToSpawn = LevelManager.Instance.GameCardsGiven[i].MeshPrefab;
                Instantiate(prefabToSpawn, _spawnPoint.transform.position, Quaternion.identity);
            }

            //Add score to text
            _currentScore += LevelManager.Instance.PointsAwarded[i];
            _scoreText.text = SCORE_TEXT + SCORE_ANIM + _currentScore;

            yield return new WaitForSeconds(_delayBTWspawns);
        }

        yield return new WaitForSeconds(_delayBeforeLoadScene);
        OnGameCardSpawnEnded?.Invoke();
    }
}
