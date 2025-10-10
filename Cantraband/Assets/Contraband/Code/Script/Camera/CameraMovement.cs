using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [Header("Shaking Values")]
    [SerializeField] private float _baseRadius = 3f;
    [SerializeField] private float _angleSpeed = 1f;
    [SerializeField] private float _radiusVariation = 1f;

    [Header("Noise settings")]
    [SerializeField] private float _noiseScale = 1f;
    [SerializeField] private float _noiseOffset = 0f;

    private Vector2 centerPoint;
    private float angle;

    private void Awake()
    {
        centerPoint = transform.position;
    }

    void Update()
    {
        angle += _angleSpeed * Time.deltaTime;

        float time = Time.time * _noiseScale + _noiseOffset;
        float noisyRadius = _baseRadius + Mathf.PerlinNoise(time, 0f) * _radiusVariation;

        float x = centerPoint.x + Mathf.Cos(angle) * noisyRadius;
        float y = centerPoint.y + Mathf.Sin(angle) * noisyRadius;

        transform.position = new Vector3(x, y, transform.position.z);
    }
}
