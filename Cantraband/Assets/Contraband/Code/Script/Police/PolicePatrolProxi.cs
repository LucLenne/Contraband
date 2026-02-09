using UnityEngine;

public class PolicePatrolProxi : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PolicePatrol _policePatrol;

    public void LaunchCheckPlayerCoatInAnim() => _policePatrol.LaunchCheckPlayerCoatInAnim();
    public void LaunchStopPatrol() => _policePatrol.StopPatrol();

    public void LaunchGameOver()
    {
        if (LevelManager.Instance == null)
            return;

        LevelManager.Instance.LaunchGameOver(true);
    }
}
