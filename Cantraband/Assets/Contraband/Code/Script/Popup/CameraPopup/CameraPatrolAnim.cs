using UnityEngine;

public class CameraPatrolAnim : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private string _camDeathTriggerName;

    private void Awake()
    {
        gameObject.SetActive(false);
    }

    public void EndAnim()
    {
        gameObject.SetActive(false);
    }

    public void CheckPlayerCoat()
    {
        if (PopUpManager.Instance.LaunchCheckPlayerCoat())
            _animator.SetTrigger(_camDeathTriggerName);
    }
}
