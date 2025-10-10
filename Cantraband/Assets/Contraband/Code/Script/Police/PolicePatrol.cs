using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class PolicePatrol : MonoBehaviour
{
    private const string SOUND_PATROL = "Fol_walk";
    private const string ANIMATION_LOOKING_PLAYER_BOOL = "Looking";
    private const string ANIMATION_RESET = "Reset";

    [Header("References")]
    [SerializeField] private GameObject _animationObject;
    [SerializeField] private Animator _animator;

    [Header("Parameters")]
    [SerializeField, Range(0f,1f)] private float _lookingAtPlayerChance;

    private void Awake()
    {
        _animationObject.SetActive(false);
    }

    private void OnEnable()
    {
        PopUpManager.Instance.OnLaunchPolicePatrol += ActivatePatrol;
        if(LevelManager.Instance != null)
        {
            LevelManager.Instance.OnGameOver += StopAnimation;
        }
        
    }

    private void OnDisable()
    {

        PopUpManager.Instance.OnLaunchPolicePatrol -= ActivatePatrol;
        if(LevelManager.Instance != null)
            LevelManager.Instance.OnGameOver -= StopAnimation;
    }

    public void ActivatePatrol()
    {
        _animationObject.SetActive(true);

        bool _isLookingAtPlayer = Random.value >= _lookingAtPlayerChance;
        _animator.SetBool(ANIMATION_LOOKING_PLAYER_BOOL, _isLookingAtPlayer);
        _animator.SetTrigger(ANIMATION_RESET);

        AudioManager.AudioManager.Instance.PlaySound(SOUND_PATROL);
    }

    public void LaunchCheckPlayerCoatInAnim()
    {
        if (LevelManager.Instance == null) return;

        LevelManager.Instance.CheckPlayerCoat();
        LevelManager.Instance.CheckPlayerTransaction();
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
