using UnityEngine;

public class LoadingAnimationProxi : MonoBehaviour
{
    [SerializeField] private LoadingManager _loadingManager;

    public void HideCanvas() => _loadingManager.HideCanvas();
}
