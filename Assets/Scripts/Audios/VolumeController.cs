using UnityEngine;
using UnityEngine.Audio;
using static Volume;

public class VolumeController: MonoBehaviour
{
    [SerializeField] private AudioMixer mixer;
    
    public void SetBGMVolume(float volume)
    {
        SaveBGMVolumes(volume);
        float BGMVolume = AudioVolumeHelper.ConvertVolumeScale_dB(volume);
        mixer.SetFloat("BGMVolume", BGMVolume);
    }
}
