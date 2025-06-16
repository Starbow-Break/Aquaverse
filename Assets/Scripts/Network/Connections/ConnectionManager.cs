using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fusion;
using Fusion.Addons.Physics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using static ConnectionData.ConnectionTarget;

public class ConnectionManager : MonoBehaviour
{
    public static ConnectionManager Instance { get; private set; }
    
    public bool IsGameHost() => _gameConnection.IsRunning && _gameConnection.Runner.IsServer;
    public ConnectionContainer GetLobbyConnection() => _lobbyConnection;
    public ConnectionContainer GetGameConnection() => _gameConnection;
    
    [SerializeField] private App _app;
    [SerializeField] private ConnectionData _defaultLobby;
    
    private ConnectionContainer _lobbyConnection = new ConnectionContainer();
    private ConnectionContainer _gameConnection = new ConnectionContainer();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(this);
        }
    }
    
    public void LoadGameLevel(ConnectionData data)
    {
        StartCoroutine(LoadSceneProcess(data));
    }

    private IEnumerator LoadSceneProcess(ConnectionData data)
    {
        var cc = data.Target == Lobby ? _lobbyConnection : _gameConnection;
        if (!cc.IsRunning) yield break;
        if (!cc.Runner.IsServer) yield break;

        yield return new WaitUntil(() => cc.App);
        cc.App.RPC_ShutdownRunner(data.Target);
        
        if (cc.Runner.IsServer)
        {
            cc.Runner.LoadScene(SceneRef.FromIndex(cc.ActiveConnection.SceneIndex));
        }
    }

    public async Task ConnectToRunner(ConnectionData connectionData, Action<NetworkRunner> onInitialized = default, Action<ShutdownReason> onFailed = default)
    {
        var connection = connectionData.Target == Lobby ? _lobbyConnection : _gameConnection;
        connection.ActiveConnection = connectionData;

        var gameMode = connectionData.Target == Lobby ? GameMode.Shared : GameMode.AutoHostOrClient;
        
        var sceneInfo = new NetworkSceneInfo();
        if (connectionData.Target == Lobby)
        {
            sceneInfo.AddSceneRef(SceneRef.FromIndex(connectionData.SceneIndex));
        }
        
        var sessionProperties = new Dictionary<string, SessionProperty>()
            { { "ID", (int)connectionData.ID } };
        
        if (connection.Runner == default)
        {
            var child = new GameObject(connection.ActiveConnection.ID.ToString());
            child.transform.SetParent(transform);
            connection.Runner = child.AddComponent<NetworkRunner>();
            connection.Runner.AddComponent<RunnerSimulatePhysics3D>();
        }

        if (connection.Callback == default)
        {
            connection.Callback = new ConnectionCallbacks();
        }

        if (connectionData.Target == Game)
        {
            connection.Callback.ActionOnShutdown += OnGameShutdown;
            connection.Callback.ActionOnPlayerJoined += OnGamePlayerJoined;
            connection.Callback.ActionOnPlayerLeft += OnGamePlayerLeft;
        }
        connection.Callback.ActionOnSceneLoadDone += OnSceneLoadDone;
        connection.Callback.ActionOnSceneLoadStart += OnSceneLoadStart;

        if (connection.IsRunning)
        {
            await connection.Runner.Shutdown();
        }
        
        if (connectionData.Target == Lobby && _gameConnection.IsRunning)
        {
            await _gameConnection.Runner.Shutdown();
        }
        
        connection.Runner.AddCallbacks(connection.Callback);
        
        onInitialized += runner =>
        {
            if (runner.IsServer || runner.IsSharedModeMasterClient)
            {
                connection.App = runner.Spawn(_app);
            }
            
            if (runner.GameMode == GameMode.Client)
            {
                var lobby = GetLobbyConnection().Runner;
                if (lobby != null && lobby.IsRunning)
                {
                    _ = lobby.Shutdown();
                }
            }
        };
        
        var startResult = await connection.Runner.StartGame(new StartGameArgs()
        { 
            GameMode = gameMode,
            SessionProperties = sessionProperties,
            EnableClientSessionCreation = true,
            Scene = sceneInfo, PlayerCount = connectionData.MaxClients,
            OnGameStarted = onInitialized,
            SceneManager = connection.Runner.gameObject.AddComponent<NetworkSceneManagerDefault>(),
        });

        if (!startResult.Ok)
            onFailed?.Invoke(startResult.ShutdownReason);
    }
    
    private void OnGameShutdown(NetworkRunner runner, ShutdownReason reason)
    {
        if (reason == ShutdownReason.DisconnectedByPluginLogic)
        {
            _ = ConnectToRunner(_defaultLobby);
        }

        InterfaceManager.Instance.ClearInterface();
    }

    private void OnGamePlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        if (!runner.IsServer)
            return;
        
        StartCoroutine(SpawnPlayerAfterSceneLoad());
    }
    
    private IEnumerator SpawnPlayerAfterSceneLoad()
    {
        yield return new WaitUntil(() => SceneManager.GetActiveScene().name == "RoomScene");
        
        var spawner = FindAnyObjectByType<RoomStarter>();
        
        if (spawner != null)
        {
            spawner.AutoHostOrClientSpawn();
        }
        else
        {
            Debug.LogError("PlayerSpawner를 찾을 수 없습니다!");
        }
    }
    
    private void OnGamePlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        if (!runner.IsServer)
            return;

        if (runner.ActivePlayers.Count() == 1)
        {
            Debug.Log("모든 플레이어가 나갔습니다. 게임을 종료합니다.");
            _ = runner.Shutdown(true, ShutdownReason.DisconnectedByPluginLogic);
        }
    }

    private void OnSceneLoadDone(NetworkRunner runner)
    {
        var sceneIndex = SceneManager.GetActiveScene().buildIndex;
        AudioManager.Instance.PlayBGM((BGMType)sceneIndex);
    }
    
    private void OnSceneLoadStart(NetworkRunner runner)
    {
        AudioManager.Instance.StopBGM();
    }
}
