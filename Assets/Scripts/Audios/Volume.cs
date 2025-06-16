using UnityEngine;

public static class Volume
{
    public static float BGMVolume = 0.5f;
    
    public static void SaveBGMVolumes(float volume)
    {
        BGMVolume = volume;
        PlayerPrefs.SetFloat("BGMVolume", BGMVolume);
        PlayerPrefs.Save();
    }

    public static void LoadVolumes()
    {
        BGMVolume = PlayerPrefs.GetFloat("BGMVolume", 0.5f);
    }
}
