using System;
using Fusion.Addons.KCC;
using UnityEngine;

public class NormalJumpProcessor : KCCProcessor, ISetDynamicVelocity
{
    [SerializeField] private Vector3 _baseJumpImpulse = 10f * Vector3.up;
    
    private readonly float DefaultPriority = 10003;
    public override float GetPriority(KCC kcc) => DefaultPriority;
    
    public void Execute(ISetDynamicVelocity stage, KCC kcc, KCCData data)
    {
        var playerData = kcc.GetComponent<PlayerMovement>().PlayerData;
        var fixedData = kcc.FixedData;
        
        if (fixedData.IsGrounded)
        {
            if (playerData.JumpTrigger.TryShot())
            {
                ApplyJump(kcc, data, playerData);
            }
            SuppressOtherJumpProcessors(kcc);
        }
        
        SuppressOtherSameTypeProcessors(kcc);
    }

    private void ApplyJump(KCC kcc, KCCData data, PlayerData playerData)
    {
        Vector3 jumpImpulse = JumpImpulseHelper.GetJumpImpulse(_baseJumpImpulse, playerData.PlayerScale);
        var fixedData = kcc.FixedData;
        fixedData.DynamicVelocity = Vector3.zero;
        fixedData.JumpImpulse = jumpImpulse;

        ApplyJumpAnimation(kcc);
    }
    
    private void ApplyJumpAnimation(KCC kcc)
    {
        var animatorController = kcc.GetComponent<NetworkAnimatorController>();
        animatorController.RPC_SetTrigger(Constant.JumpHash);
    }
    
    private void SuppressOtherSameTypeProcessors(KCC kcc)
    {
        kcc.SuppressProcessors<NormalJumpProcessor>();
    }

    private void SuppressOtherJumpProcessors(KCC kcc)
    {
        kcc.SuppressProcessors<AirJumpProcessor>();
        kcc.SuppressProcessors<WallJumpProcessor>();
    }
}
