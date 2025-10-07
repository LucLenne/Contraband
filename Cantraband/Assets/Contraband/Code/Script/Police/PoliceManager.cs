using NaughtyAttributes;
using System.Collections;
using UnityEngine;

public class PoliceManager : MonoBehaviour
{
    [Header("Parameters")]
    [SerializeField, MinMaxSlider(0f,30f)] private Vector2 _minMaxDelay;

    private Coroutine _infiniteSpawnCoroutine;

    private void Awake()
    {
        _infiniteSpawnCoroutine = StartCoroutine(SpawnRoutine());
    }

    private void OnEnable()
    {
        LevelManager.Instance.OnGameOver += StopPolice;
    }

    private void OnDisable()
    {
        LevelManager.Instance.OnGameOver -= StopPolice;
    }

    private IEnumerator SpawnRoutine()
    {
        while (true)
        {
            float _delay = Random.Range(_minMaxDelay.x, _minMaxDelay.y);
            yield return new WaitForSeconds(_delay);

            PopUpManager.Instance.SpawnRandomPolicePopup();
        }

    }

    private void StopPolice()
    {
        if (_infiniteSpawnCoroutine != null)
        {
            StopCoroutine(_infiniteSpawnCoroutine);
            _infiniteSpawnCoroutine = null;
        }
    }
}
