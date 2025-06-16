using Fusion.Addons.KCC;
using UnityEngine;

public class ScaleProcessor : KCCProcessor, ISetScale
{
    private readonly float DefaultPriority = default;
    public override float GetPriority(KCC kcc) => DefaultPriority;
    
    public void Execute(ISetScale stage, KCC kcc, KCCData data)
    {
        var fixedData = kcc.FixedData;
        
        if (!fixedData.IsGrounded) return;
        
        var playerData = kcc.GetComponent<PlayerMovement>().PlayerData;
        
        bool needChange = false;
        if (playerData.BiggerTrigger.TryShot())
        {
            needChange |= playerData.TryBigger();
        }
        else if (playerData.SmallerTrigger.TryShot())
        {
            needChange |= playerData.TrySmaller();
        }

        if (needChange)
        {
            UpdatePlayerScale(kcc, playerData);
        }
    }

    private void UpdatePlayerScale(KCC kcc, PlayerData playerData)
    {
        float targetScale = playerData.PlayerScale switch
        {
            EPlayerScale.Small => 0.5f,
            EPlayerScale.Normal => 1f,
            EPlayerScale.Big => 2f,
            _ => 1f
        };

        var playerScaler = kcc.GetComponent<PlayerScaler>();
        playerScaler?.SetScale(targetScale * Vector3.one);
    }
}
