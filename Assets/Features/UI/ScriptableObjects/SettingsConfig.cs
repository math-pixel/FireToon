using UnityEngine;
using UnityEngine.Audio;

[CreateAssetMenu(fileName = "SettingsConfig", menuName = "Game/Settings Config")]
public class SettingsConfig : ScriptableObject
{
    [Header("Default Values")]
    public float defaultMasterVolume = 0.75f;
    public float defaultMusicVolume = 0.75f;
    public float defaultSFXVolume = 0.75f;
    public float defaultMouseSensitivity = 1f;
    public bool defaultInvertY = false;
    public bool defaultFullscreen = true;
    public int defaultQualityLevel = 2;
    
    [Header("Audio Mixer Parameters")]
    public string masterVolumeParameter = "MasterVolume";
    public string musicVolumeParameter = "MusicVolume";
    public string sfxVolumeParameter = "SFXVolume";
    
    [Header("Volume Settings")]
    public float minVolume = 0.0001f; // Pour éviter log10(0)
    public float maxVolume = 1f;
    public bool useLogarithmicVolume = true;
    
    [Header("Resolution Settings")]
    public bool filterDuplicateResolutions = true;
    public int minResolutionWidth = 800;
    public int minResolutionHeight = 600;
    
    [Header("Quality Settings")]
    public string[] customQualityNames;
    public bool useCustomQualityNames = false;
    
    [Header("PlayerPrefs Keys")]
    public string masterVolumeKey = "MasterVolume";
    public string musicVolumeKey = "MusicVolume";
    public string sfxVolumeKey = "SFXVolume";
    public string mouseSensitivityKey = "MouseSensitivity";
    public string invertYKey = "InvertY";
    public string fullscreenKey = "Fullscreen";
    public string qualityLevelKey = "QualityLevel";
    public string resolutionIndexKey = "ResolutionIndex";
    
    public float ConvertToMixerValue(float sliderValue)
    {
        if (!useLogarithmicVolume) return sliderValue;
        
        // Clamp to avoid log10(0)
        sliderValue = Mathf.Clamp(sliderValue, minVolume, maxVolume);
        return Mathf.Log10(sliderValue) * 20;
    }
    
    public float ConvertFromMixerValue(float mixerValue)
    {
        if (!useLogarithmicVolume) return mixerValue;
        
        return Mathf.Pow(10, mixerValue / 20);
    }
}
