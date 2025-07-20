using System;
using Fusion.Addons.KCC;
using UnityEngine;

public class WallJumpProcessor : KCCProcessor, ISetDynamicVelocity
{
    [SerializeField] private Vector3 _baseJumpImpulse = 10f * Vector3.up;
    [SerializeField] private float _wallSlideMultiplier = 0.5f;
    
    private readonly float DefaultPriority = 10001;
    public override float GetPriority(KCC kcc) => DefaultPriority;
    
    public void Execute(ISetDynamicVelocity stage, KCC kcc, KCCData data)
    {
        var playerData = kcc.GetComponent<PlayerMovement>().PlayerData;
        var fixedData = kcc.FixedData;
        if (!playerData.Wall) return;
        
        if (playerData.JumpTrigger.TryShot())
        {
            ApplyWallJump(kcc, data, playerData);
            SuppressOtherJumpProcessors(kcc);
        }
        else
        {
            fixedData.DynamicVelocity *= _wallSlideMultiplier;
        }

        SuppressOtherSameTypeProcessors(kcc);
    }

    private void ApplyWallJump(KCC kcc, KCCData data, PlayerData playerData)
    {
        Quaternion rot = Quaternion.LookRotation(playerData.WallNormal, Vector3.up);
        Vector3 jumpImpulse = JumpImpulseHelper.GetJumpImpulse(_baseJumpImpulse, playerData.PlayerScale);
        
        var fixedData = kcc.FixedData;
        playerData.Wall = false;
        playerData.WallNormal = default;
        fixedData.DynamicVelocity = Vector3.zero;
        fixedData.JumpImpulse = rot * jumpImpulse;
        
        kcc.AddLookRotation(new Vector2(0f, 180f));

        ApplyJumpAnimation(kcc);
    }
    
    private void ApplyJumpAnimation(KCC kcc)
    {
        var animatorController = kcc.GetComponent<NetworkAnimatorController>();
        animatorController.RPC_SetTrigger(Constant.JumpHash);
    }

    private void SuppressOtherSameTypeProcessors(KCC kcc)
    {
        kcc.SuppressProcessors<WallJumpProcessor>();
        
    }

    private void SuppressOtherJumpProcessors(KCC kcc)
    {
        kcc.SuppressProcessors<NormalJumpProcessor>();
        kcc.SuppressProcessors<AirJumpProcessor>();
    }
}
