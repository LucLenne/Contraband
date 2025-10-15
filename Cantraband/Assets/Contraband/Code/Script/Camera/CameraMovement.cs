using System;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

public class CameraMovement : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PopUpManager _popUpManager;

    [Header("Shaking Values")]
    [SerializeField] private float _baseRadius = 3f;
    [SerializeField] private float _angleSpeed = 1f;
    [SerializeField] private float _radiusVariation = 1f;

    [Header("Noise settings")]
    [SerializeField] private float _noiseScale = 1f;
    [SerializeField] private float _noiseOffset = 0f;

    [Header("Camera popup")]
    [SerializeField] private Camera _parentCamera;
    [SerializeField] private float _mostLeftXPosition;
    [SerializeField] private float _mostRightXPosition;
    [SerializeField] private float _widthWhenBothSize;
    [SerializeField] private float _rectLerpSpeed = .9f;
    [Space(5)]
    [SerializeField] private Color _parentCameraColor;

    private Vector2 centerPoint;
    private float angle;

    private Rect _targetRect;
    private bool _hasTopLeftPopup;
    private bool _hasLowerLeftPopup;
    private bool _hasRightPopup;

    private float _leftTopProgress = 0f;
    private float _leftBottomProgress = 0f;
    private float _rightProgress = 0f;

    private void Awake()
    {
        centerPoint = transform.position;

        //Setup camera
        Camera bgCam = new GameObject("BackgroundCamera").AddComponent<Camera>();
        bgCam.clearFlags = CameraClearFlags.SolidColor;
        bgCam.backgroundColor = Color.black;
        bgCam.cullingMask = 0; // rien à rendre
        bgCam.depth = -1;
        bgCam.backgroundColor = _parentCameraColor;

        _parentCamera.clearFlags = CameraClearFlags.Depth;
        _parentCamera.depth = 0;
        _targetRect = _parentCamera.rect;
    }

    private void OnEnable()
    {
        _popUpManager.OnNewPopup += ChangeParentCameraMovement;
        _popUpManager.OnLeavePopup += RemoveParentCameraMovement;
        _popUpManager.OnUpdateProgress += UpdateParentCameraMovement;
    }

    private void OnDisable()
    {
        _popUpManager.OnNewPopup -= ChangeParentCameraMovement;
        _popUpManager.OnLeavePopup -= RemoveParentCameraMovement;
        _popUpManager.OnUpdateProgress -= UpdateParentCameraMovement;
    }

    void Update()
    {
        //Constant client movement
        angle += _angleSpeed * Time.deltaTime;

        float time = Time.time * _noiseScale + _noiseOffset;
        float noisyRadius = _baseRadius + Mathf.PerlinNoise(time, 0f) * _radiusVariation;

        float x = centerPoint.x + Mathf.Cos(angle) * noisyRadius;
        float y = centerPoint.y + Mathf.Sin(angle) * noisyRadius;

        transform.position = new Vector3(x, y, transform.position.z);


        //Parent camera viewport movement
        float t = Mathf.Clamp01(_rectLerpSpeed * Time.deltaTime);
        Rect current = _parentCamera.rect;
        Rect newRect = LerpRect(current, _targetRect, t);
        _parentCamera.rect = newRect;
    }

    private void ChangeParentCameraMovement(PopUpManager.AngleType type)
    {
        switch (type)
        {
            case PopUpManager.AngleType.UpperLeft:
                _hasTopLeftPopup = true;
                break;

            case PopUpManager.AngleType.LowerLeft:
                _hasLowerLeftPopup = true;
                break;

            case PopUpManager.AngleType.UpperRight:
                _hasRightPopup = true;
                break;

            case PopUpManager.AngleType.LowerRight:
                throw new NotImplementedException();
        }
    }

    private void RemoveParentCameraMovement(PopUpManager.AngleType type)
    {
        switch (type)
        {
            case PopUpManager.AngleType.UpperLeft:
                _hasTopLeftPopup = false;
                _leftTopProgress = 0f;
                break;

            case PopUpManager.AngleType.LowerLeft:
                _hasLowerLeftPopup = false;
                _leftBottomProgress = 0f;
                break;

            case PopUpManager.AngleType.UpperRight:
                _hasRightPopup = false;
                _rightProgress = 0f;
                break;

            case PopUpManager.AngleType.LowerRight:
                throw new NotImplementedException();
        }
        ProcessParentCameraMovement();
    }

    void ProcessParentCameraMovement()
    {
        if ((_hasTopLeftPopup || _hasLowerLeftPopup) && _hasRightPopup)
        {
            _targetRect = new Rect(
                0.5f - _widthWhenBothSize / 2f,
                0f,
                _widthWhenBothSize,
                1f
            );
        }
        else if (_hasTopLeftPopup || _hasLowerLeftPopup)
            _targetRect = new Rect(_mostLeftXPosition, 0f, 1f, 1f);
        else if (_hasRightPopup)
            _targetRect = new Rect(_mostRightXPosition, 0f, 1f, 1f);
        else
            _targetRect = new Rect(0f, 0f, 1f, 1f);
    }

    private void UpdateParentCameraMovement(PopUpManager.AngleType type, float progress)
    {
        // clamp de la progression
        progress = Mathf.Clamp01(progress);

        // Met à jour la progression selon le type reçu
        switch (type)
        {
            case PopUpManager.AngleType.UpperLeft:
                _leftTopProgress = progress;
                break;

            case PopUpManager.AngleType.LowerLeft:
                _leftBottomProgress = progress;
                break;

            case PopUpManager.AngleType.UpperRight:
            case PopUpManager.AngleType.LowerRight:
                _rightProgress = progress;
                break;
        }

        // Calcul du rect cible en fonction des progressions
        bool leftActive = _leftTopProgress + _leftBottomProgress > 0f;
        bool rightActive = _rightProgress > 0f;

        if (leftActive && rightActive)
        {
            // Les deux côtés s'ouvrent : on réduit la largeur vers _widthWhenBothSize
            // On prend la progression commune comme le min des deux (les deux doivent être ouverts)
            float bothProgress = Mathf.Max(Mathf.Max(_leftTopProgress, _leftBottomProgress), _rightProgress);
            float width = Mathf.Lerp(1f, _widthWhenBothSize, bothProgress);
            float x = 0.5f - width / 2f;
            _targetRect = new Rect(x, 0f, width, 1f);
        }
        else if (leftActive)
        {
            // Seulement gauche -> décalage vers la droite
            float leftProgress = Mathf.Max(_leftTopProgress, _leftBottomProgress);
            float x = Mathf.Lerp(0f, _mostLeftXPosition, leftProgress);
            _targetRect = new Rect(x, 0f, 1f, 1f);
        }
        else if (rightActive)
        {
            // Seulement droite -> décalage vers la gauche
            float x = Mathf.Lerp(0f, _mostRightXPosition, _rightProgress);
            _targetRect = new Rect(x, 0f, 1f, 1f);
        }
        else
        {
            // Aucun popup -> rect plein
            _targetRect = new Rect(0f, 0f, 1f, 1f);
        }
    }

    private Rect LerpRect(Rect a, Rect b, float t)
    {
        return new Rect(
            Mathf.Lerp(a.x, b.x, t),
            Mathf.Lerp(a.y, b.y, t),
            Mathf.Lerp(a.width, b.width, t),
            Mathf.Lerp(a.height, b.height, t)
        );
    }
}
