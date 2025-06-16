using System;
using System.Collections.Generic;
using System.Linq;
using Fusion;
using Fusion.Sockets;
using UnityEngine;

public class ConnectionCallbacks : INetworkRunnerCallbacks {
    public Action<NetworkRunner, PlayerRef> ActionOnPlayerJoined;
    public Action<NetworkRunner, PlayerRef> ActionOnPlayerLeft;
    public Action<NetworkRunner, ShutdownReason> ActionOnShutdown;
    public Action<NetworkRunner> ActionOnSceneLoadDone;
    public Action<NetworkRunner> ActionOnSceneLoadStart;

    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        Debug.Log($"{player} Joined");
        Debug.Log($"total players: {runner.ActivePlayers.Count()}");
        
        ActionOnPlayerJoined?.Invoke(runner, player);
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        ActionOnPlayerLeft?.Invoke(runner, player);
    }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
        ActionOnShutdown?.Invoke(runner, shutdownReason);
    }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        var inputs = new MyNetworkInput();

        inputs.LookYaw = InputTracker.Instance.LookYaw;
        inputs.LookPitch = InputTracker.Instance.LookPitch;
        
        if (Input.GetKey(KeyCode.W)) {
            inputs.Buttons.Set(MyNetworkInput.BUTTON_FORWARD, true);
        }
        
        if (Input.GetKey(KeyCode.S)) {
            inputs.Buttons.Set(MyNetworkInput.BUTTON_BACKWARD, true);
        }
        
        if (Input.GetKey(KeyCode.A)) {
            inputs.Buttons.Set(MyNetworkInput.BUTTON_LEFT, true);
        }
        
        if (Input.GetKey(KeyCode.D)) {
            inputs.Buttons.Set(MyNetworkInput.BUTTON_RIGHT, true);
        }
        
        if (InputTracker.Instance.GetKeyDown(KeyCode.E)) {
            Debug.Log("E");
            inputs.Buttons.Set(MyNetworkInput.BUTTON_INTERACT, true);
        }
        
        if (InputTracker.Instance.GetKeyDown(KeyCode.Q)) {
            Debug.Log("Q");
            inputs.Buttons.Set(MyNetworkInput.BUTTON_END_INTERACT, true);
        }

        if (Input.GetKey(KeyCode.LeftShift))
        {
            inputs.Buttons.Set(MyNetworkInput.BUTTON_RUN, true);
        }
        
        if (InputTracker.Instance.GetKeyDown(KeyCode.Space))
        {
            inputs.Buttons.Set(MyNetworkInput.BUTTON_JUMP, true);
        }
        
        if (InputTracker.Instance.GetKeyDown(KeyCode.Mouse0))
        {
            inputs.Buttons.Set(MyNetworkInput.BUTTON_LEFTCLICK, true);
        }
        
        if (InputTracker.Instance.GetKeyDown(KeyCode.Mouse1))
        {
            inputs.Buttons.Set(MyNetworkInput.BUTTON_RIGHTCLICK, true);
        }

        // inputs.forward = transform.forward;
        
        input.Set(inputs);

        InputTracker.Instance.Clear();
    }
    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) {}
    public void OnConnectedToServer(NetworkRunner runner) {}
    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason) { }
    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) {}
    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) {}
    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) {}
    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) {}
    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) {}
    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) {}
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data) { }

    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) { }

    public void OnSceneLoadDone(NetworkRunner runner)
    {
        ActionOnSceneLoadDone?.Invoke(runner);
    }
    public void OnSceneLoadStart(NetworkRunner runner)
    {
        ActionOnSceneLoadStart?.Invoke(runner);
    }
}
