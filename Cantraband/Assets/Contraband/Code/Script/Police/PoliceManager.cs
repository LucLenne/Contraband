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

    private IEnumerator SpawnRoutine()
    {
        while (true)
        {
            float _delay = Random.Range(_minMaxDelay.x, _minMaxDelay.y);
            yield return new WaitForSeconds(_delay);

            Debug.LogWarning("Check if is between transactions");
 
            PopUpManager.Instance.SpawnRandomPolicePopup();
        }

    }
}
