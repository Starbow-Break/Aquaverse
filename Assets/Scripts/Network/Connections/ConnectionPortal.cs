using System;
using UnityEngine;
using Fusion;

public class ConnectionPortal : MonoBehaviour
{
    [SerializeField] private ConnectionData _connectionData;
    
    public static event Action<ConnectionData> OnPortalTriggered;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponentInParent<NetworkObject>() == null)
            return;
        
        if (other.GetComponentInParent<NetworkObject>().HasInputAuthority)
        {
            InterfaceManager.Instance.GateUI.Focus();
            OnPortalTriggered?.Invoke(_connectionData);
        }
    }
}
