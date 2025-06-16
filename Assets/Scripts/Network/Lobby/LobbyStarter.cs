using UnityEngine;
using Fusion;

public class LobbyStarter : NetworkBehaviour {
    [SerializeField] private NetworkPrefabRef _playerPrefab;

    public override void Spawned()
    {
        InterfaceManager.Instance.MouseDisable();
        
        Runner.Spawn(_playerPrefab, Vector3.up * 3, inputAuthority: Runner.LocalPlayer);
    }
}