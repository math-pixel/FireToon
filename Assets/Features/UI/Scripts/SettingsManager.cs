using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using System;
using System.Collections.Generic;
using System.Linq;

public class SettingsManager : MonoBehaviour
{
    [Header("Configuration")] public SettingsConfig settingsConfig;

    [Header("Audio Settings")] public AudioMixer audioMixer;
    public Slider masterVolumeSlider;
    public Slider musicVolumeSlider;
    public Slider sfxVolumeSlider;

    [Header("Graphics Settings")] public Dropdown qualityDropdown;
    public Toggle fullscreenToggle;
    public Dropdown resolutionDropdown;

    [Header("Gameplay Settings")] public Slider mouseSensitivitySlider;
    public Toggle invertYToggle;

    [Header("UI Elements")] public Button resetButton;
    public Button saveButton;
    public Button closeButton;

    [Header("Callbacks")] public Action<float> onMasterVolumeChanged;
    public Action<float> onMusicVolumeChanged;
    public Action<float> onSFXVolumeChanged;
    public Action<int> onQualityChanged;
    public Action<bool> onFullscreenChanged;
    public Action<float> onMouseSensitivityChanged;
    public Action<bool> onInvertYChanged;
    public Action onSettingsSaved;
    public Action onSettingsReset;

    private Resolution[] resolutions;
    private List<Resolution> filteredResolutions;
    private bool isInitialized = false;

    public bool ignoreSettings = false;
    
    void Start()
    {
        if (!ignoreSettings)
        {
            InitializeSettings();
            LoadSettings();
            SetupUICallbacks();
        }
        isInitialized = true;
    }

    private void InitializeSettings()
    {
        if (settingsConfig == null)
        {
            Debug.LogError("SettingsConfig is not assigned!");
            return;
        }

        InitializeResolutions();
        InitializeQuality();
        InitializeAudioSliders();
    }

    private void InitializeResolutions()
    {
        if (resolutionDropdown == null) return;

        resolutions = Screen.resolutions;
        filteredResolutions = new List<Resolution>();

        // Filter resolutions
        foreach (var res in resolutions)
        {
            if (res.width >= settingsConfig.minResolutionWidth &&
                res.height >= settingsConfig.minResolutionHeight)
            {
                // Filter duplicates if enabled
                if (settingsConfig.filterDuplicateResolutions)
                {
                    bool isDuplicate = filteredResolutions.Any(r =>
                        r.width == res.width && r.height == res.height);
                    if (!isDuplicate)
                    {
                        filteredResolutions.Add(res);
                    }
                }
                else
                {
                    filteredResolutions.Add(res);
                }
            }
        }

        // Setup dropdown
        resolutionDropdown.ClearOptions();
        var options = new List<string>();
        int currentResolutionIndex = 0;

        for (int i = 0; i < filteredResolutions.Count; i++)
        {
            var res = filteredResolutions[i];
            string option = $"{res.width} x {res.height}";

            // Add refresh rate if multiple resolutions with same dimensions
            if (filteredResolutions.Count(r => r.width == res.width && r.height == res.height) > 1)
            {
                option += $" @ {res.refreshRate}Hz";
            }

            options.Add(option);

            if (res.width == Screen.currentResolution.width &&
                res.height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
    }

    private void InitializeQuality()
    {
        if (qualityDropdown == null) return;

        qualityDropdown.ClearOptions();

        if (settingsConfig.useCustomQualityNames &&
            settingsConfig.customQualityNames != null &&
            settingsConfig.customQualityNames.Length > 0)
        {
            qualityDropdown.AddOptions(settingsConfig.customQualityNames.ToList());
        }
        else
        {
            qualityDropdown.AddOptions(QualitySettings.names.ToList());
        }

        qualityDropdown.value = QualitySettings.GetQualityLevel();
        qualityDropdown.RefreshShownValue();
    }

    private void InitializeAudioSliders()
    {
        // Set slider ranges
        if (masterVolumeSlider != null)
        {
            masterVolumeSlider.minValue = settingsConfig.minVolume;
            masterVolumeSlider.maxValue = settingsConfig.maxVolume;
        }

        if (musicVolumeSlider != null)
        {
            musicVolumeSlider.minValue = settingsConfig.minVolume;
            musicVolumeSlider.maxValue = settingsConfig.maxVolume;
        }

        if (sfxVolumeSlider != null)
        {
            sfxVolumeSlider.minValue = settingsConfig.minVolume;
            sfxVolumeSlider.maxValue = settingsConfig.maxVolume;
        }
    }

    private void SetupUICallbacks()
    {
        // Audio sliders
        masterVolumeSlider?.onValueChanged.AddListener(SetMasterVolume);
        musicVolumeSlider?.onValueChanged.AddListener(SetMusicVolume);
        sfxVolumeSlider?.onValueChanged.AddListener(SetSFXVolume);

        // Graphics
        qualityDropdown?.onValueChanged.AddListener(SetQuality);
        fullscreenToggle?.onValueChanged.AddListener(SetFullscreen);
        resolutionDropdown?.onValueChanged.AddListener(SetResolution);

        // Gameplay
        mouseSensitivitySlider?.onValueChanged.AddListener(SetMouseSensitivity);
        invertYToggle?.onValueChanged.AddListener(SetInvertY);

        // Buttons
        resetButton?.onClick.AddListener(ResetToDefaults);
        saveButton?.onClick.AddListener(SaveSettings);
        closeButton?.onClick.AddListener(CloseSettings);
    }

    public void LoadSettings()
    {
        if (settingsConfig == null) return;

        // Load and apply audio settings
        float masterVolume = PlayerPrefs.GetFloat(settingsConfig.masterVolumeKey, settingsConfig.defaultMasterVolume);
        float musicVolume = PlayerPrefs.GetFloat(settingsConfig.musicVolumeKey, settingsConfig.defaultMusicVolume);
        float sfxVolume = PlayerPrefs.GetFloat(settingsConfig.sfxVolumeKey, settingsConfig.defaultSFXVolume);

        if (masterVolumeSlider != null)
        {
            masterVolumeSlider.value = masterVolume;
        }

        SetMasterVolumeInternal(masterVolume);

        if (musicVolumeSlider != null)
        {
            musicVolumeSlider.value = musicVolume;
        }

        SetMusicVolumeInternal(musicVolume);

        if (sfxVolumeSlider != null)
        {
            sfxVolumeSlider.value = sfxVolume;
        }

        SetSFXVolumeInternal(sfxVolume);

        // Load graphics settings
        int qualityLevel = PlayerPrefs.GetInt(settingsConfig.qualityLevelKey, settingsConfig.defaultQualityLevel);
        if (qualityDropdown != null)
        {
            qualityDropdown.value = qualityLevel;
        }

        QualitySettings.SetQualityLevel(qualityLevel);

        bool fullscreen = PlayerPrefs.GetInt(settingsConfig.fullscreenKey, settingsConfig.defaultFullscreen ? 1 : 0) ==
                          1;
        if (fullscreenToggle != null)
        {
            fullscreenToggle.isOn = fullscreen;
        }

        Screen.fullScreen = fullscreen;

        // Load resolution
        int resolutionIndex = PlayerPrefs.GetInt(settingsConfig.resolutionIndexKey, 0);
        if (resolutionDropdown != null && resolutionIndex < filteredResolutions.Count)
        {
            resolutionDropdown.value = resolutionIndex;
        }

        // Load gameplay settings
        float mouseSensitivity =
            PlayerPrefs.GetFloat(settingsConfig.mouseSensitivityKey, settingsConfig.defaultMouseSensitivity);
        if (mouseSensitivitySlider != null)
        {
            mouseSensitivitySlider.value = mouseSensitivity;
        }

        bool invertY = PlayerPrefs.GetInt(settingsConfig.invertYKey, settingsConfig.defaultInvertY ? 1 : 0) == 1;
        if (invertYToggle != null)
        {
            invertYToggle.isOn = invertY;
        }
    }

    // Audio Settings
    public void SetMasterVolume(float volume)
    {
        if (!isInitialized) return;
        SetMasterVolumeInternal(volume);
        PlayerPrefs.SetFloat(settingsConfig.masterVolumeKey, volume);
        onMasterVolumeChanged?.Invoke(volume);
    }

    private void SetMasterVolumeInternal(float volume)
    {
        if (audioMixer != null && settingsConfig != null)
        {
            float mixerValue = settingsConfig.ConvertToMixerValue(volume);
            audioMixer.SetFloat(settingsConfig.masterVolumeParameter, mixerValue);
        }
    }

    public void SetMusicVolume(float volume)
    {
        if (!isInitialized) return;
        SetMusicVolumeInternal(volume);
        PlayerPrefs.SetFloat(settingsConfig.musicVolumeKey, volume);
        onMusicVolumeChanged?.Invoke(volume);
    }

    private void SetMusicVolumeInternal(float volume)
    {
        if (audioMixer != null && settingsConfig != null)
        {
            float mixerValue = settingsConfig.ConvertToMixerValue(volume);
            audioMixer.SetFloat(settingsConfig.musicVolumeParameter, mixerValue);
        }
    }

    public void SetSFXVolume(float volume)
    {
        if (!isInitialized) return;
        SetSFXVolumeInternal(volume);
        PlayerPrefs.SetFloat(settingsConfig.sfxVolumeKey, volume);
        onSFXVolumeChanged?.Invoke(volume);
    }

    private void SetSFXVolumeInternal(float volume)
    {
        if (audioMixer != null && settingsConfig != null)
        {
            float mixerValue = settingsConfig.ConvertToMixerValue(volume);
            audioMixer.SetFloat(settingsConfig.sfxVolumeParameter, mixerValue);
        }
    }

    // Graphics Settings
    public void SetQuality(int qualityIndex)
    {
        if (!isInitialized) return;
        QualitySettings.SetQualityLevel(qualityIndex);
        PlayerPrefs.SetInt(settingsConfig.qualityLevelKey, qualityIndex);
        onQualityChanged?.Invoke(qualityIndex);
    }

    public void SetFullscreen(bool isFullscreen)
    {
        if (!isInitialized) return;
        Screen.fullScreen = isFullscreen;
        PlayerPrefs.SetInt(settingsConfig.fullscreenKey, isFullscreen ? 1 : 0);
        onFullscreenChanged?.Invoke(isFullscreen);
    }

    public void SetResolution(int resolutionIndex)
    {
        if (!isInitialized || filteredResolutions == null || resolutionIndex >= filteredResolutions.Count) return;

        Resolution resolution = filteredResolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen, resolution.refreshRate);
        PlayerPrefs.SetInt(settingsConfig.resolutionIndexKey, resolutionIndex);
    }

    // Gameplay Settings
    public void SetMouseSensitivity(float sensitivity)
    {
        if (!isInitialized) return;
        PlayerPrefs.SetFloat(settingsConfig.mouseSensitivityKey, sensitivity);
        onMouseSensitivityChanged?.Invoke(sensitivity);
    }

    public void SetInvertY(bool invert)
    {
        if (!isInitialized) return;
        PlayerPrefs.SetInt(settingsConfig.invertYKey, invert ? 1 : 0);
        onInvertYChanged?.Invoke(invert);
    }

    // Utility methods
    public void ResetToDefaults()
    {
        if (settingsConfig == null) return;

        // Reset audio
        SetMasterVolumeInternal(settingsConfig.defaultMasterVolume);
        SetMusicVolumeInternal(settingsConfig.defaultMusicVolume);
        SetSFXVolumeInternal(settingsConfig.defaultSFXVolume);

        // Reset graphics
        QualitySettings.SetQualityLevel(settingsConfig.defaultQualityLevel);
        Screen.fullScreen = settingsConfig.defaultFullscreen;

        // Update UI without triggering callbacks
        isInitialized = false;

        if (masterVolumeSlider != null)
            masterVolumeSlider.value = settingsConfig.defaultMasterVolume;
        if (musicVolumeSlider != null)
            musicVolumeSlider.value = settingsConfig.defaultMusicVolume;
        if (sfxVolumeSlider != null)
            sfxVolumeSlider.value = settingsConfig.defaultSFXVolume;
        if (mouseSensitivitySlider != null)
            mouseSensitivitySlider.value = settingsConfig.defaultMouseSensitivity;
        if (invertYToggle != null)
            invertYToggle.isOn = settingsConfig.defaultInvertY;
        if (fullscreenToggle != null)
            fullscreenToggle.isOn = settingsConfig.defaultFullscreen;
        if (qualityDropdown != null)
            qualityDropdown.value = settingsConfig.defaultQualityLevel;

        isInitialized = true;

        // Save defaults to PlayerPrefs
        PlayerPrefs.SetFloat(settingsConfig.masterVolumeKey, settingsConfig.defaultMasterVolume);
        PlayerPrefs.SetFloat(settingsConfig.musicVolumeKey, settingsConfig.defaultMusicVolume);
        PlayerPrefs.SetFloat(settingsConfig.sfxVolumeKey, settingsConfig.defaultSFXVolume);
        PlayerPrefs.SetFloat(settingsConfig.mouseSensitivityKey, settingsConfig.defaultMouseSensitivity);
        PlayerPrefs.SetInt(settingsConfig.invertYKey, settingsConfig.defaultInvertY ? 1 : 0);
        PlayerPrefs.SetInt(settingsConfig.fullscreenKey, settingsConfig.defaultFullscreen ? 1 : 0);
        PlayerPrefs.SetInt(settingsConfig.qualityLevelKey, settingsConfig.defaultQualityLevel);

        // Trigger callbacks
        onMasterVolumeChanged?.Invoke(settingsConfig.defaultMasterVolume);
        onMusicVolumeChanged?.Invoke(settingsConfig.defaultMusicVolume);
        onSFXVolumeChanged?.Invoke(settingsConfig.defaultSFXVolume);
        onQualityChanged?.Invoke(settingsConfig.defaultQualityLevel);
        onFullscreenChanged?.Invoke(settingsConfig.defaultFullscreen);
        onMouseSensitivityChanged?.Invoke(settingsConfig.defaultMouseSensitivity);
        onInvertYChanged?.Invoke(settingsConfig.defaultInvertY);

        onSettingsReset?.Invoke();

        Debug.Log("Settings reset to defaults!");
    }

    public void SaveSettings()
    {
        PlayerPrefs.Save();
        onSettingsSaved?.Invoke();
        Debug.Log("Settings saved!");
    }

    public void CloseSettings()
    {
        // Auto-save when closing
        SaveSettings();

        // Hide settings panel (if this is attached to the settings panel)
        gameObject.SetActive(false);
    }

    // Public getters for current values
    public float GetMasterVolume()
    {
        return PlayerPrefs.GetFloat(settingsConfig?.masterVolumeKey ?? "MasterVolume",
            settingsConfig?.defaultMasterVolume ?? 0.75f);
    }

    public float GetMusicVolume()
    {
        return PlayerPrefs.GetFloat(settingsConfig?.musicVolumeKey ?? "MusicVolume",
            settingsConfig?.defaultMusicVolume ?? 0.75f);
    }

    public float GetSFXVolume()
    {
        return PlayerPrefs.GetFloat(settingsConfig?.sfxVolumeKey ?? "SFXVolume",
            settingsConfig?.defaultSFXVolume ?? 0.75f);
    }

    public float GetMouseSensitivity()
    {
        return PlayerPrefs.GetFloat(settingsConfig?.mouseSensitivityKey ?? "MouseSensitivity",
            settingsConfig?.defaultMouseSensitivity ?? 1f);
    }

    public bool GetInvertY()
    {
        return PlayerPrefs.GetInt(settingsConfig?.invertYKey ?? "InvertY",
            settingsConfig?.defaultInvertY == true ? 1 : 0) == 1;
    }

    public bool GetFullscreen()
    {
        return PlayerPrefs.GetInt(settingsConfig?.fullscreenKey ?? "Fullscreen",
            settingsConfig?.defaultFullscreen == true ? 1 : 0) == 1;
    }

    public int GetQualityLevel()
    {
        return PlayerPrefs.GetInt(settingsConfig?.qualityLevelKey ?? "QualityLevel",
            settingsConfig?.defaultQualityLevel ?? 2);
    }

    // Validation methods
    public bool ValidateSettings()
    {
        bool isValid = true;

        if (audioMixer == null)
        {
            Debug.LogWarning("AudioMixer is not assigned!");
            isValid = false;
        }

        if (settingsConfig == null)
        {
            Debug.LogError("SettingsConfig is not assigned!");
            isValid = false;
        }

        return isValid;
    }

    // Test methods for debugging
    [ContextMenu("Test Audio Settings")]
    public void TestAudioSettings()
    {
        if (audioMixer == null || settingsConfig == null) return;

        Debug.Log("Testing audio settings...");

        // Test master volume
        float testVolume = 0.5f;
        float mixerValue = settingsConfig.ConvertToMixerValue(testVolume);
        audioMixer.SetFloat(settingsConfig.masterVolumeParameter, mixerValue);

        Debug.Log($"Set master volume to {testVolume} (mixer value: {mixerValue})");
    }

    [ContextMenu("Print Current Settings")]
    public void PrintCurrentSettings()
    {
        Debug.Log("=== CURRENT SETTINGS ===");
        Debug.Log($"Master Volume: {GetMasterVolume()}");
        Debug.Log($"Music Volume: {GetMusicVolume()}");
        Debug.Log($"SFX Volume: {GetSFXVolume()}");
        Debug.Log($"Mouse Sensitivity: {GetMouseSensitivity()}");
        Debug.Log($"Invert Y: {GetInvertY()}");
        Debug.Log($"Fullscreen: {GetFullscreen()}");
        Debug.Log($"Quality Level: {GetQualityLevel()}");
        Debug.Log($"Resolution: {Screen.currentResolution.width}x{Screen.currentResolution.height}");
    }

    // Event cleanup
    void OnDestroy()
    {
        // Remove all listeners to prevent memory leaks
        masterVolumeSlider?.onValueChanged.RemoveAllListeners();
        musicVolumeSlider?.onValueChanged.RemoveAllListeners();
        sfxVolumeSlider?.onValueChanged.RemoveAllListeners();
        qualityDropdown?.onValueChanged.RemoveAllListeners();
        fullscreenToggle?.onValueChanged.RemoveAllListeners();
        resolutionDropdown?.onValueChanged.RemoveAllListeners();
        mouseSensitivitySlider?.onValueChanged.RemoveAllListeners();
        invertYToggle?.onValueChanged.RemoveAllListeners();
        resetButton?.onClick.RemoveAllListeners();
        saveButton?.onClick.RemoveAllListeners();
        closeButton?.onClick.RemoveAllListeners();
    }

    // Static utility methods
    public static SettingsManager FindSettingsManager()
    {
        return FindObjectOfType<SettingsManager>();
    }

    public static void ApplySettingsToNewScene()
    {
        SettingsManager manager = FindSettingsManager();
        if (manager != null)
        {
            manager.LoadSettings();
        }
    }
}