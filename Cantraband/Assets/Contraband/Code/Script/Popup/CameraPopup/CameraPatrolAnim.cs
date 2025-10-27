using UnityEngine;

public class CameraPatrolAnim : MonoBehaviour
{
    private void Awake()
    {
        gameObject.SetActive(false);
    }

    public void EndAnim()
    {
        gameObject.SetActive(false);
    }
}
