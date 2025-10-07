using UnityEngine;

public class PopUpProxi : MonoBehaviour
{
    [SerializeField] private PopUpGeneric _popUpScript;
    public void LaunchEventInAnim() => _popUpScript.LaunchEventInAnim();

    public void LaunchEndAnim() => _popUpScript.LaunchEndAnim();
}
