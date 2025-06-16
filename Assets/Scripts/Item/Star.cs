using System;
using Fusion;
using UnityEngine;

public class Star : NetworkBehaviour, IItem
{
    [SerializeField] private LayerMask _interactorLayer;

    public event Action<GameObject> OnGet;
    
    public void TryGet(GameObject target)
    {
        bool isInteractable = (_interactorLayer & 1 << target.layer) > 0;
        if (isInteractable && HasStateAuthority)
        {
            Get(target);
        }
    }
    
    public void Get(GameObject target)
    {
        Debug.Log("*");
        OnGet?.Invoke(target);
        RPC_Destroy();
    }
    
    [Rpc(RpcSources.All, RpcTargets.StateAuthority, HostMode = RpcHostMode.SourceIsHostPlayer)]
    public void RPC_Destroy()
    {
        RPC_DoDestroy();
    }
    
    [Rpc(RpcSources.StateAuthority, RpcTargets.All, HostMode = RpcHostMode.SourceIsServer)]
    private void RPC_DoDestroy()
    {
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        TryGet(other.gameObject);
    }
}

