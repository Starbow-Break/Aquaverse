using Fusion;
using Fusion.Addons.KCC;
using UnityEngine;

public class PlayerRespawner : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        var networkObject = other.GetComponentInParent<NetworkObject>();
        
        if (networkObject == null || !networkObject.HasStateAuthority)
            return;
        
        var cc = other.GetComponentInParent<KCC>();
        RespawnPlayer(cc);
    }

    private void RespawnPlayer(KCC cc)
    {
        var respawnPoint = transform;
        
        cc.TeleportRPC(respawnPoint.position, respawnPoint.rotation.eulerAngles.y,
            respawnPoint.rotation.eulerAngles.x);
    }
}
