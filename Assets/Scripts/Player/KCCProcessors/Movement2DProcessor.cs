using Fusion;
using Fusion.Addons.KCC;

public class Movement2DProcessor: KCCProcessor, ISetDynamicVelocity, ISetKinematicVelocity 
{
    private readonly float DefaultPriority = -10000;
    
    public override float GetPriority(KCC kcc) => DefaultPriority;
    
    public void Execute(ISetDynamicVelocity stage, KCC kcc, KCCData data)
    {
        data.DynamicVelocity = data.DynamicVelocity.OnlyXY();
    }

    public void Execute(ISetKinematicVelocity stage, KCC kcc, KCCData data)
    {
        data.KinematicVelocity = data.KinematicVelocity.OnlyXY();
    }
}