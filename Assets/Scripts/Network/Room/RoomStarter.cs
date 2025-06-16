using UnityEngine;
using Fusion;
using Fusion.Addons.KCC;

public class RoomStarter : NetworkBehaviour {
    [SerializeField] private NetworkPrefabRef _playerPrefab;
    
    [SerializeField] private Transform _spawnPointA;
    [SerializeField] private Transform _spawnPointB;
    
    public override void Spawned()
    {
        InterfaceManager.Instance.MouseEnable();
    }

    public void AutoHostOrClientSpawn()
    {
        bool isHost = Runner.SessionInfo.PlayerCount == 1;
        var spawnPoint = isHost ? _spawnPointA : _spawnPointB;
        var spawnPosition = spawnPoint.position;
        var spawnRotation = spawnPoint.rotation;
        
        foreach (PlayerRef playerRef in Runner.ActivePlayers)
        {
            if (Runner.SessionInfo.PlayerCount == 2 && playerRef == Runner.LocalPlayer)
                continue;
            
            var playerObject = Runner.Spawn(_playerPrefab, spawnPosition, spawnRotation, inputAuthority: playerRef);
            
            RPC_ComponentDisabled(playerObject);
        }
    }
    
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_ComponentDisabled(NetworkObject playerObject)
    {
        Debug.Log("55");
        playerObject.GetComponent<PlayerMovement>().enabled = false;
        playerObject.GetComponent<KCC>().enabled = false;
    }
}
