using Fusion;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class NetworkAnimatorController : NetworkBehaviour
{
    private Animator _animator;

    public Animator Animator => _animator;
    
    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    #region RPC Send
    [Rpc(RpcSources.All, RpcTargets.StateAuthority, HostMode = RpcHostMode.SourceIsHostPlayer)]
    public void RPC_SetFloat(int hash, float value)
    {
        RPC_ApplySetFloat(hash, value);
    }
    
    [Rpc(RpcSources.All, RpcTargets.StateAuthority, HostMode = RpcHostMode.SourceIsHostPlayer)]
    public void RPC_SetInteger(int hash, int value)
    {
        RPC_ApplySetInteger(hash, value);
    }
    
    [Rpc(RpcSources.All, RpcTargets.StateAuthority, HostMode = RpcHostMode.SourceIsHostPlayer)]
    public void RPC_SetBool(int hash, bool value)
    {
        RPC_ApplySetBool(hash, value);
    }
    
    [Rpc(RpcSources.All, RpcTargets.StateAuthority, HostMode = RpcHostMode.SourceIsHostPlayer)]
    public void RPC_SetTrigger(int hash)
    {
        RPC_ApplySetTrigger(hash);
    }
    #endregion
    
    #region RPC Apply
    [Rpc(RpcSources.StateAuthority, RpcTargets.All, HostMode = RpcHostMode.SourceIsServer)]
    public void RPC_ApplySetFloat(int hash, float value)
    {
        _animator.SetFloat(hash, value);
    }
    
    [Rpc(RpcSources.StateAuthority, RpcTargets.All, HostMode = RpcHostMode.SourceIsServer)]
    public void RPC_ApplySetInteger(int hash, int value)
    {
        _animator.SetInteger(hash, value);
    }
    
    [Rpc(RpcSources.StateAuthority, RpcTargets.All, HostMode = RpcHostMode.SourceIsServer)]
    public void RPC_ApplySetBool(int hash, bool value)
    {
        _animator.SetBool(hash, value);
    }
    
    [Rpc(RpcSources.StateAuthority, RpcTargets.All, HostMode = RpcHostMode.SourceIsServer)]
    public void RPC_ApplySetTrigger(int hash)
    {
        _animator.SetTrigger(hash);
    }
    #endregion
}
