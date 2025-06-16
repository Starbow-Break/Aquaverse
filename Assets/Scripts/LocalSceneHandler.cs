using UnityEngine;
using UnityEngine.SceneManagement;
using Cysharp.Threading.Tasks;

public class LocalSceneHandler : MonoBehaviour
{
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    
    public async void OnClick(string sceneName)
    {
        AudioManager.Instance.StopBGM();
        
        await new WaitUntil(() => AudioManager.Instance.StopBGMCoroutine == null);
        
        SceneManager.LoadScene(sceneName);
    }

    public void OnClickExit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
    
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        var sceneIndex = scene.buildIndex;
        AudioManager.Instance.PlayBGM((BGMType)sceneIndex);
    }
}
