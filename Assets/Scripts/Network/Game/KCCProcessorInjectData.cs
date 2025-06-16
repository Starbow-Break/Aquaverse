using System;
using Fusion.Addons.KCC;

[Serializable]
public struct KCCProcessorInjectData
{
    [Serializable, Flags]
    public enum EInjectTarget
    {
        Host = 1 << 0,
        Client = 1 << 1
    }
    
    public KCCProcessor Processor;
    public EInjectTarget InjectTarget;
}
