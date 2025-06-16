using Fusion.Addons.KCC;

public class GroundActProcessor : KCCProcessor, IAfterMoveStep
{
    private readonly float DefaultPriority = -5000;
    public override float GetPriority(KCC kcc) => DefaultPriority;
    
    public void Execute(AfterMoveStep stage, KCC kcc, KCCData data)
    {
        var playerData = kcc.GetComponent<PlayerMovement>().PlayerData;

        if (data.IsGrounded)
        {
            playerData.ResetAirJump();
        }

        ApplyIsGroundedAnimation(kcc, data.IsGrounded);
    }
    
    private void ApplyIsGroundedAnimation(KCC kcc, bool isGrounded)
    {
        var animatorController = kcc.GetComponent<NetworkAnimatorController>();
        animatorController.RPC_SetBool(Constant.IsGroundedHash, isGrounded);
    }
}
