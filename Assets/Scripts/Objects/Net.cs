using Fusion;
using UnityEngine;

public class Net : NetworkBehaviour
{
    [Networked] private float TargetScaleX { get; set; }
    
    [SerializeField] private float scaleSpeed = 1f;
    
    public override void Spawned()
    {
        if (HasStateAuthority)
        {
            TargetScaleX = transform.localScale.x;
        }
    }
    
    public override void FixedUpdateNetwork()
    {
        if (HasStateAuthority)
        {
            Vector3 scale = transform.localScale;
            scale.x = Mathf.MoveTowards(scale.x, TargetScaleX, scaleSpeed * Runner.DeltaTime);
            transform.localScale = scale;
        }
    }
    
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RpcShrink()
    {
        TargetScaleX = 0f;
    }
}
