using Fusion;
using UnityEngine;

public class Elevator : NetworkBehaviour
{
    [Networked] private float TargetY { get; set; }
    [SerializeField] private float speed;
    [SerializeField] private float topY;
    [SerializeField] private float bottomY;
    [SerializeField] private bool isUp;

    private Vector3 _startPos;

    public override void Spawned()
    {
        _startPos = transform.position;
        if (HasStateAuthority)
        {
            TargetY = _startPos.y;
        }
    }

    public override void FixedUpdateNetwork()
    {
        if (HasStateAuthority)
        {
            Vector3 pos = transform.position;
            pos.y = Mathf.MoveTowards(pos.y, TargetY, speed * Runner.DeltaTime);
            transform.position = pos;
        }
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RpcMoveElevator()
    {
        isUp = !isUp;
        TargetY = isUp ? topY : bottomY;
    }
}
