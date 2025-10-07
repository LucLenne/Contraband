using NaughtyAttributes;
using System.Collections;
using UnityEngine;

public class PoliceManager : MonoBehaviour
{
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
            float _delay = Random.Range(Mathf.Max(RythmManager.Instance.PopUpMinMaxDelay.x,0), 
                                        Mathf.Max(RythmManager.Instance.PopUpMinMaxDelay.y,0));
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
