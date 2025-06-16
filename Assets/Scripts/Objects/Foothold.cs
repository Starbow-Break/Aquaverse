using Fusion;
using UnityEngine;

public class Foothold : NetworkBehaviour
{
    [Networked] private float TargetX { get; set; }
    
    [SerializeField] private float speed;
    [SerializeField] private float moveX;
    
    private Vector3 _startPos;
    
    public override void Spawned()
    {
        _startPos = transform.position;
        if (HasStateAuthority)
        {
            TargetX = _startPos.x;
        }
    }
    
    public override void FixedUpdateNetwork()
    {
        if (HasStateAuthority)
        {
            Vector3 pos = transform.position;
            pos.x = Mathf.MoveTowards(pos.x, TargetX, speed * Runner.DeltaTime);
            transform.position = pos;
        }
    }
    
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RpcMoveFoothold()
    {
        TargetX = moveX;
    }
}
