using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Collections;
using System.Threading.Tasks;

public class EscUI : UIScreen
{
    public static event Action OnMenuClosedByButton;
    
    [SerializeField] private ConnectionData _lobbyConnectionData;
    
    public async void OnClickQuit()
    {
        OnMenuClosedByButton?.Invoke();
        Defocus();
        
        Scene activeScene = SceneManager.GetActiveScene();

        if (activeScene.name == "LobbyScene")
        {
            SceneManager.LoadScene("StartScene");
            
            var lobbyConnection = ConnectionManager.Instance.GetLobbyConnection();
            var lobbyRunner  = lobbyConnection.Runner;

            if (lobbyRunner != null && lobbyRunner.IsRunning)
            {
                await lobbyRunner.Shutdown();
            }
            
            InterfaceManager.Instance.MouseEnable();
        }
        else
        {
            await ConnectionManager.Instance.ConnectToRunner(_lobbyConnectionData);
        }
    }

    public void OnClickContinue()
    {
        OnMenuClosedByButton?.Invoke();
        Defocus();
    }
}