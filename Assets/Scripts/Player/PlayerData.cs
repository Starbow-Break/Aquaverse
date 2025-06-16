using Fusion;
using UnityEngine;

public class PlayerData
{
    // Values
    public int PlayerID { get; private set; }
    public bool Running { get; set; }
    public bool Wall { get; set; }

    public Vector3 WallNormal { get; set; }
    public int AirJump { get; private set; }
    public EPlayerScale PlayerScale { get; private set; }
    
    public Trigger JumpTrigger { get; private set; }
    public Trigger BiggerTrigger { get; private set; }
    public Trigger SmallerTrigger { get; private set; }

    // Constructor
    public PlayerData(int playerID)
    {
        PlayerID = playerID;
        Running = false;
        Wall = false;
        AirJump = 1;
        PlayerScale = EPlayerScale.Normal;
        
        JumpTrigger = new Trigger();
        BiggerTrigger = new Trigger();
        SmallerTrigger = new Trigger();
    }

    public void ReleaseAllTrigger()
    {
        JumpTrigger.Release();
        BiggerTrigger.Release();
        SmallerTrigger.Release();
    }
    
    public void ApplyAirJump()
    {
        AirJump--;
    }
    
    public void ResetAirJump()
    {
        AirJump = 1;
    }

    public bool TrySmaller()
    {
        if (PlayerScale == EPlayerScale.Small) return false;
        
        PlayerScale = PlayerScale switch
        {
            EPlayerScale.Big => EPlayerScale.Normal,
            _ => EPlayerScale.Small
        };

        return true;
    }

    public bool TryBigger()
    {
        if (PlayerScale == EPlayerScale.Big) return false;
        
        PlayerScale = PlayerScale switch
        {
            EPlayerScale.Small => EPlayerScale.Normal,
            _ => EPlayerScale.Big
        };

        return true;
    }
}
