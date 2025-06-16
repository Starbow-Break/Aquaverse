using Fusion;
using UnityEngine;

public class ClearUI : UIScreen
{
    [SerializeField] private ConnectionData _lobbyConnectionData;
    
    public async void OnClickQuit()
    {
        await ConnectionManager.Instance.ConnectToRunner(_lobbyConnectionData);

        Defocus();
    }
}
