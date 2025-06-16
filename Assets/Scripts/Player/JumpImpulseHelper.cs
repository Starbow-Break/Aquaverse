using UnityEngine;

public static class JumpImpulseHelper
{
    private static readonly float ImpulseMultiplierSmall = 1 / Mathf.Sqrt(2f);
    private static readonly float ImpulseMultiplierBig = 0.25f;
    
    public static Vector3 GetJumpImpulse(Vector3 baseImpulse, EPlayerScale playerScale)
    {
        Vector3 jumpImpulse = playerScale switch
        {
            EPlayerScale.Small => ImpulseMultiplierSmall * baseImpulse,
            EPlayerScale.Big => ImpulseMultiplierBig * baseImpulse,
            _ => baseImpulse
        };

        return jumpImpulse;
    }
}
