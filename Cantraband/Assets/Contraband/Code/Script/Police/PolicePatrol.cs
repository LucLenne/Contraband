using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class PolicePatrol : MonoBehaviour
{
    private const string SOUND_PATROL = "Fol_walk";
    private const string ANIMATION_LOOKING_PLAYER_BOOL = "Looking";
    private const string ANIMATION_RESET = "Reset";
    private const string ANIMATION_SPOTTED = "PlayerSpotted";

    [Header("References")]
    [SerializeField] private GameObject _animationObject;
    [SerializeField] private Animator _animator;

    [Header("Parameters")]
    [SerializeField] private bool _activateFake;
    [SerializeField, Range(0f,1f)] private float _lookingAtPlayerChance;

    [Header("Audio")]
    [SerializeField] private AudioSource _source;
    [SerializeField] private AudioClip _clip;

    private void Awake()
    {
        _source.playOnAwake = false;
        _source.clip = _clip;
        _source.loop = true;
        _source.Stop();

        _animationObject.SetActive(false);
    }

    private void OnEnable()
    {
        PopUpManager.Instance.OnLaunchPolicePatrol += ActivatePatrol;
        InputManager.Instance.OnVestChanged += CheckPlayHeartBeat;
        if(LevelManager.Instance != null)
        {
            LevelManager.Instance.OnGameOver += StopAnimation;
        }

    }

    private void OnDisable()
    {
        PopUpManager.Instance.OnLaunchPolicePatrol -= ActivatePatrol;
        InputManager.Instance.OnVestChanged -= CheckPlayHeartBeat;
        if(LevelManager.Instance != null)
            LevelManager.Instance.OnGameOver -= StopAnimation;
    }

    private void CheckPlayHeartBeat(bool isOpened)
    {
        if (!_animationObject.activeSelf)
            return;

        if (!isOpened)
            _source.Play();
        else 
            _source.Stop();
    }

    public void ActivatePatrol()
    {
        _animationObject.SetActive(true);

        bool _isLookingAtPlayer = Random.value >= _lookingAtPlayerChance;
        if (!_activateFake)
            _isLookingAtPlayer = true;

        _animator.SetBool(ANIMATION_LOOKING_PLAYER_BOOL, _isLookingAtPlayer);
        _animator.SetTrigger(ANIMATION_RESET);

        AudioManager.AudioManager.Instance.PlaySound(SOUND_PATROL);
    }

    public void LaunchCheckPlayerCoatInAnim()
    {
        if (LevelManager.Instance == null) 
            return;

        if(LevelManager.Instance.CheckPlayerCoat(false) || LevelManager.Instance.CheckPlayerTransaction(false))
        {
            LevelManager.Instance.OnSpottedByCop?.Invoke();

            _animator.SetTrigger(ANIMATION_SPOTTED);
        }
    }

    public void StopPatrol()
    {
        _animationObject.SetActive(false);
    }

    private void StopAnimation()
    {
        _animator.speed = 0;
    }
}
