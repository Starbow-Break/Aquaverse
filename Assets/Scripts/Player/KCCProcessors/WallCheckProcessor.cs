using System;
using Fusion.Addons.KCC;
using UnityEngine;

public class WallCheckProcessor : KCCProcessor, ISetDynamicVelocity
{
    [SerializeField] private float _castDistance = 1f;
    [SerializeField] private LayerMask _wallLayer;
    
    private readonly float DefaultPriority = 10002;
    public override float GetPriority(KCC kcc) => DefaultPriority;

    private KCC _kcc;
    
    public void Execute(ISetDynamicVelocity stage, KCC kcc, KCCData data)
    {
        _kcc = kcc;
        
        var playerData = kcc.GetComponent<PlayerMovement>().PlayerData;
        var fixedData = kcc.FixedData;
        
        if (fixedData.IsGrounded)
        {
            playerData.Wall = false;
            ApplyWallAnimation(kcc, false);
            return;
        }

        bool result = Physics.BoxCast(
            kcc.transform.position + kcc.Collider.height / 2f * Vector3.up,
            0.1f * new Vector3(kcc.Collider.radius, kcc.Collider.height / 2f, 1f),
            kcc.transform.forward,
            out RaycastHit hit,
            kcc.transform.rotation, 
            kcc.Collider.radius + _castDistance,
            _wallLayer
        );

        if (result && data.RealVelocity.y < 0f && Vector3.Angle(Vector3.up, hit.normal) > data.MaxGroundAngle)
        {
            playerData.Wall = true;
            playerData.WallNormal = hit.normal;
        }
        else
        {
            playerData.Wall = false;
            playerData.WallNormal = default;
        }
        
        ApplyWallAnimation(kcc, playerData.Wall);
    }
    
    private void ApplyWallAnimation(KCC kcc, bool isWall)
    {
        var animatorController = kcc.GetComponent<NetworkAnimatorController>();
        animatorController.RPC_SetBool(Constant.IsWallHash, isWall);
    }
}
