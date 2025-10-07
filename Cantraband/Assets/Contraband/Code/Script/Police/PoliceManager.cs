using NaughtyAttributes;
using System.Collections;
using UnityEngine;

public class PoliceManager : MonoBehaviour
{
    [Header("Parameters")]
    [SerializeField, MinMaxSlider(0f,10f)] private Vector2 _minMaxDelay;

    private Coroutine _infiniteSpawnCoroutine;

    private void Awake()
    {
        _infiniteSpawnCoroutine = StartCoroutine(SpawnRoutine());
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
}
