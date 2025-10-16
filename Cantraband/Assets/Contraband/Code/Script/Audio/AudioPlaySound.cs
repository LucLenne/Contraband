using UnityEngine;
using static AudioManager.AudioManager;

public class AudioPlaySound : MonoBehaviour
{
    [SerializeField] private string _soundName;
    [Tooltip("Leave -1 to be random")]
    [SerializeField] private int _audioDataIndex = -1;
    [Space(10)]
    [SerializeField] private bool _playOnEnable;

    private void OnEnable()
    {
        AudioManager.AudioManager.Instance.PlaySound(_soundName);
        //if (_playOnEnable)
        //{
        //    PlaySound();
        //}
    }

    public void PlaySound()
    {
        if (AudioManager.AudioManager.Instance)
        {
            AudioManager.AudioManager.Instance.PlaySound(_soundName);
        }
    }
}
