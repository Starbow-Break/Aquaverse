using Fusion;
using UnityEngine;

public struct MyNetworkInput : INetworkInput
{
    public const int BUTTON_FORWARD = 0;
    public const int BUTTON_BACKWARD = 1;
    public const int BUTTON_LEFT = 2;
    public const int BUTTON_RIGHT = 3;
    public const int BUTTON_INTERACT = 4;
    public const int BUTTON_END_INTERACT = 5;
    public const int BUTTON_JUMP = 6;
    public const int BUTTON_RUN = 7;
    public const int BUTTON_LEFTCLICK = 8;
    public const int BUTTON_RIGHTCLICK = 9;
    
    
    public NetworkButtons Buttons;

    public float LookYaw;
    public float LookPitch;

    public bool IsUp(int button) {
        return Buttons.IsSet(button) == false;
    }

    public bool IsDown(int button) {
        return Buttons.IsSet(button);
    }
}
