using UnityEngine;

public static class AudioVolumeHelper
{
    public static float ConvertVolumeScale_dB(float volumeScaleValue, float lowerBound = float.MinValue)
    {
        if (Mathf.Approximately(volumeScaleValue, 0f))
        {
            return lowerBound;
        }
        
        return Mathf.Max(lowerBound, Mathf.Log10(volumeScaleValue) * 20);
    }
}