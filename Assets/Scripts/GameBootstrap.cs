using Fusion;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameBootstrap : MonoBehaviour
{
    public NetworkRunner runnerPrefab;
    private NetworkRunner runnerInstance;
    
    public void StartAsHost()
    {
        StartGame(GameMode.Host);
    }

    public void StartAsClient()
    {
        StartGame(GameMode.Client);
    }
    
    private async void StartGame(GameMode mode)
    {
        runnerInstance = Instantiate(runnerPrefab);
        runnerInstance.ProvideInput = true;

        await runnerInstance.StartGame(new StartGameArgs
        {
            GameMode = mode,
            SessionName = "Game",
            Scene = SceneRef.FromIndex(SceneManager.GetActiveScene().buildIndex),
            SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>()
        });
    }
}
