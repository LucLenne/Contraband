using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameOverScene : MonoBehaviour
{
    [System.Serializable]  
    private struct SoundStruct
    {
        public AudioClip clip;
        public float delay;
    }

    private const string SCORE_TEXT = "Score\n";
    private const string SCORE_ANIM = "<+spread><wave><palette><-fade>";

    [Header("References")]
    [SerializeField] private Transform _spawnPoint;
    [SerializeField] private GameObject _musicPlayer;
    [SerializeField] private TMP_Text _scoreText;

    [Header("Delay")]
    [SerializeField] private float _delayBTWspawns;
    [SerializeField] private float _delayBeforeLoadScene;

    [Header("Sounds")]
    [SerializeField] private AudioSource _source;
    [SerializeField] private List<SoundStruct> _notes;

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

        float _numberOfCartridges = LevelManager.Instance.GameCardsGiven.Count;
        for (int i = 0; i < LevelManager.Instance.score; i++)
        {
            //Spawn game cards
            int index = (int)(i % _numberOfCartridges);
            int soundIndex = (int)(i % _notes.Count);
            GameObject prefabToSpawn = LevelManager.Instance.GameCardsGiven[index].MeshPrefab;
            Instantiate(prefabToSpawn, _spawnPoint.transform.position, Random.rotation);

            SoundStruct currentNote = _notes[soundIndex];
            _source.clip = currentNote.clip;
            _source.Play();

            //Add score to text
            _currentScore++;
            _scoreText.text = SCORE_TEXT + SCORE_ANIM + _currentScore;

            yield return new WaitForSeconds(currentNote.delay);
        }

        yield return new WaitForSeconds(_delayBeforeLoadScene);
        OnGameCardSpawnEnded?.Invoke();
    }
}
