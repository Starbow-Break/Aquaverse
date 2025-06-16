using Cysharp.Threading.Tasks;
using Fusion;
using UnityEngine;

public class App : NetworkBehaviour
{
    public override void Spawned()
    {
        DontDestroyOnLoad(this);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.StateAuthority)]
    public void RPC_ShutdownRunner(ConnectionData.ConnectionTarget target)
    {
        // InterfaceManager.Instance.ClearInterface();

        var connection = target == ConnectionData.ConnectionTarget.Lobby
            ? ConnectionManager.Instance.GetLobbyConnection()
            : ConnectionManager.Instance.GetGameConnection();
        var downConnection = target == ConnectionData.ConnectionTarget.Lobby
            ? ConnectionManager.Instance.GetGameConnection()
            : ConnectionManager.Instance.GetLobbyConnection();
        
        if (downConnection.IsRunning)
        {
            DelayShutdown(connection, downConnection).Forget();
        }
    }
    
    private async UniTaskVoid DelayShutdown(ConnectionContainer cc, ConnectionContainer downCc)
    {
        await UniTask.WaitUntil(() => cc.Runner != null && cc.IsRunning);
        await downCc.Runner.Shutdown();
    }
}