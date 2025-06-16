using Fusion.Addons.KCC;

public class GrabProcessor : KCCProcessor, ISetKinematicSpeed
{
    private readonly float DefaultPriority = default;
    public override float GetPriority(KCC kcc) => DefaultPriority;
    
    public void Execute(ISetKinematicSpeed stage, KCC kcc, KCCData data)
    {
        var grabInteractor = kcc.GetComponent<GrabInteractor>();
        var fixedData = kcc.FixedData;
        
        if (grabInteractor?.Grabbable != null)
        {
            fixedData.KinematicSpeed *= grabInteractor.Grabbable.SpeedMultiplier;
        }
    }
}
