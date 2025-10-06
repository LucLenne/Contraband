using System;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PopUpGeneric : MonoBehaviour
{
    [Header("Parameters")]
    [SerializeField] private float _speed = 1f;

    private Animator _animator;

    public float Speed 
    { 
        get => _speed; 
        set => _speed = value; 
    }

    public Action OnPhase1Completed;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _animator.speed = _speed;
    }

    public void LaunchPhase1Anim()
    {
        OnPhase1Completed?.Invoke();
    }

    public void LaunchEndAnim()
    {
        Destroy(gameObject);
    }
}
