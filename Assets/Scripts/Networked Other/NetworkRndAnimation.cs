using Fusion;
using UnityEngine;

public class NetworkRndAnimation : NetworkBehaviour, IAfterSpawned
{
    private Animator _anim;

    [SerializeField] private string _titleAnim;
    
    [Networked, OnChangedRender(nameof(OnUpdateAnim))]
    private float AnimOffset { get; set; }

    public override void Spawned()
    {
        _anim = GetComponent<Animator>();
    }

    public void AfterSpawned()
    {
        if (Object.HasStateAuthority)
        {
            AnimOffset = Random.Range(0f, 1f);
        }
        else
        {
            Rpc_UpdateAnim();
        }
    }
    
    [Rpc(RpcSources.All, RpcTargets.StateAuthority, HostMode = RpcHostMode.SourceIsHostPlayer)]
    private void Rpc_UpdateAnim()
    {
        var animStateInfo = _anim.GetCurrentAnimatorStateInfo(0);
        float newOffset = animStateInfo.normalizedTime % 1f;
        AnimOffset = newOffset;
    }

    private void OnUpdateAnim()
    {
        Debug.Log(AnimOffset);
        _anim.Play(_titleAnim, 0, AnimOffset);
    }
}
