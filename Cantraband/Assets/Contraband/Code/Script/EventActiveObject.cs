using UnityEngine;
using UnityEngine.Events;

public class EventActiveObject : MonoBehaviour
{
    [SerializeField] private UnityEvent _onEnableEvent;
    [SerializeField] private UnityEvent _onDisableEvent;
    [SerializeField] private bool _skipFirstTime;
    private void OnEnable()
    {
        if (_skipFirstTime)
        {
            _skipFirstTime = false;
            return;
        }

        _onEnableEvent?.Invoke();
    }

    private void OnDisable()
    {
#if UNITY_EDITOR
        if (!UnityEditor.EditorApplication.isPlaying)
            return;
#endif
        _onDisableEvent?.Invoke();
    }
}
