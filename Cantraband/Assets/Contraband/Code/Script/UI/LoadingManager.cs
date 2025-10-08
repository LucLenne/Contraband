using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingManager : MonoBehaviour
{
    public static LoadingManager Instance;

    [Header("Animation")]
    [SerializeField] private GameObject _loadingCanvas;
    [SerializeField] private Animator _animator;
    [SerializeField] private string _endLoadingTriggerName;

    private string _sceneToLoad;
    private Coroutine _loadingCoroutine;


    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        DontDestroyOnLoad(gameObject);
    }

    public void LoadScene(string scene)
    {
        _sceneToLoad = scene;
        _loadingCoroutine = StartCoroutine(LoadingRountine());
    }

    private IEnumerator LoadingRountine()
    {
        AsyncOperation asyncLoading = SceneManager.LoadSceneAsync(_sceneToLoad);
        _loadingCanvas.SetActive(true);

        while (!asyncLoading.isDone)
            yield return null;

        _animator.SetTrigger(_endLoadingTriggerName);
    }

    public void HideCanvas() => _loadingCanvas.SetActive(false);
}
