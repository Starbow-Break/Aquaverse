using Fusion.Addons.KCC;
using UnityEngine;

public class RunProcessor : KCCProcessor, ISetKinematicSpeed
{
    [field: SerializeField, Min(1f)] 
    public float RunMultiplier { get; private set; } = 2f;

    private readonly float DefaultPriority = default;
    public override float GetPriority(KCC kcc) => DefaultPriority;
    
    public void Execute(ISetKinematicSpeed stage, KCC kcc, KCCData data)
    {
        var playerData = kcc.GetComponent<PlayerMovement>().PlayerData;
        
        if (playerData is { Running: true })
        {
            ApplyRun(kcc);
        }
        
        SuppressOtherProcessors(kcc);
    }

    private void ApplyRun(KCC kcc)
    {
        var fixedData = kcc.FixedData;
        fixedData.KinematicSpeed *= RunMultiplier;
    }

    private void SuppressOtherProcessors(KCC kcc)
    {
        kcc.SuppressProcessors<RunProcessor>();
    }
}
