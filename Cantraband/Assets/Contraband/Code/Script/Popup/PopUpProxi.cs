using UnityEngine;

public class PopUpProxi : MonoBehaviour
{
    [SerializeField] private PopUpGeneric _popUpScript;
    public void LaunchCheckPlayerCoatInAnim() => _popUpScript.LaunchCheckPlayerCoatInAnim();

    public void LaunchPolicePatrol() => _popUpScript.LaunchPolicePatrol();

    public void LaunchEndAnim() => _popUpScript.LaunchEndAnim();

    public void LaunchTriggerAnimation(string triggerName) => _popUpScript.LaunchTrigger(triggerName);
    public void LaunchTriggerFakeCameraAnimation(string triggerName) => _popUpScript.LaunchTriggerFakeCamera(triggerName);
}
