using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Audio;

public class SettingsManager : MonoBehaviour
{
    public GameObject settingsPanel;
    public Toggle fullscreenToggle;
    public Text toggleLabel;
    public TMP_Dropdown resolutionDropdown;

    private Resolution[] availableResolutions;
    private Resolution defaultResolution;

    public Slider masterVolumeSlider;
    public Slider musicVolumeSlider;
    public Slider voiceVolumeSlider;
    public AudioMixer audioMixer;

    public Button defaultButton;
    public Button closeButton;

    public static SettingsManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        InitializeVolumes();
        InitializeResolutions();
        InitializeDisplayMode();
        
        fullscreenToggle.onValueChanged.AddListener(SetDisplayMode);
        resolutionDropdown.onValueChanged.AddListener(SetResolution);
        closeButton.onClick.AddListener(CloseSetting);
        defaultButton.onClick.AddListener(ResetSetting);
        masterVolumeSlider.onValueChanged.AddListener(SetMasterVolume);
        musicVolumeSlider.onValueChanged.AddListener(SetMusicVolume);
        voiceVolumeSlider.onValueChanged.AddListener(SetVoiceVolume);

        settingsPanel.SetActive(false);
    }

    public void ShowSettingsPanel()
    {
        settingsPanel.SetActive(true);
    }

    void InitializeResolutions()
    {
        availableResolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();

        var resolutionMap = new Dictionary<string, Resolution>();
        int currentResolutionIndex = 0;

        foreach (var res in availableResolutions)
        {
            const float aspectRatio = 16f / 9f;
            const float epsilon = 0.01f;

            if (Mathf.Abs((float)res.width / res.height - aspectRatio) > epsilon)
                continue;

            string option = res.width + "x" + res.height;
            if (!resolutionMap.ContainsKey(option))
            {
                resolutionMap[option] = res;
                resolutionDropdown.options.Add(new TMP_Dropdown.OptionData(option));
                if (res.width == Screen.currentResolution.width && res.height == Screen.currentResolution.height)
                {
                    currentResolutionIndex = resolutionDropdown.options.Count - 1;
                    defaultResolution = res;
                }
            }
        }

        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
    }

    void InitializeDisplayMode()
    {    
        fullscreenToggle.isOn = Screen.fullScreenMode == FullScreenMode.FullScreenWindow;
        UpdateToggleLabel(fullscreenToggle.isOn);
    }

    void InitializeVolumes()
    {
        masterVolumeSlider.value = PlayerPrefs.GetFloat(Constants.MASTER_VOLUME, Constants.DEFAULT_VOLUME);
        musicVolumeSlider.value = PlayerPrefs.GetFloat(Constants.MUSIC_VOLUME, Constants.DEFAULT_VOLUME);
        voiceVolumeSlider.value = PlayerPrefs.GetFloat(Constants.VOICE_VOLUME, Constants.DEFAULT_VOLUME);

        SetMasterVolume(masterVolumeSlider.value);
        SetMusicVolume(musicVolumeSlider.value);
        SetVoiceVolume(voiceVolumeSlider.value);
    }

    void SetDisplayMode(bool isFullscreen)
    {
        Screen.fullScreenMode = isFullscreen ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed;
        UpdateToggleLabel(isFullscreen);
    }

    void UpdateToggleLabel(bool isFullscreen)
    {
        toggleLabel.text = isFullscreen ? "Fullscreen" : "Windowed";
    }

    void SetResolution(int index)
    {
        string[] dimensions = resolutionDropdown.options[index].text.Split('x');
        int width = int.Parse(dimensions[0].Trim());
        int height = int.Parse(dimensions[1].Trim());
        Screen.SetResolution(width, height, Screen.fullScreenMode);
    }

    void CloseSetting()
    {
        SaveSettings();
        settingsPanel.SetActive(false);
    }

    void SaveSettings()
    {
        PlayerPrefs.SetInt("Resolution", resolutionDropdown.value);
        PlayerPrefs.SetInt("Fullscreen", fullscreenToggle.isOn ? 1 : 0);
        
        PlayerPrefs.SetFloat(Constants.MASTER_VOLUME, masterVolumeSlider.value);
        PlayerPrefs.SetFloat(Constants.MUSIC_VOLUME, musicVolumeSlider.value);
        PlayerPrefs.SetFloat(Constants.VOICE_VOLUME, voiceVolumeSlider.value);
        PlayerPrefs.Save();
    }

    private float SliderValueToDecibel(float sliderValue)
    {
        return sliderValue > 0.0001f ? Mathf.Log10(sliderValue) * 20 : -80f;
    }
    void SetMasterVolume(float volume)
    {
        audioMixer.SetFloat(Constants.MASTER_VOLUME, SliderValueToDecibel(volume));
    }

    void SetMusicVolume(float volume)
    {
        audioMixer.SetFloat(Constants.MUSIC_VOLUME, SliderValueToDecibel(volume));
    }

    void SetVoiceVolume(float volume)
    {
        audioMixer.SetFloat(Constants.VOICE_VOLUME, SliderValueToDecibel(volume));
    }


    void ResetSetting()
    {
        resolutionDropdown.value = resolutionDropdown.options.FindIndex(
            option => option.text == $"{defaultResolution.width}x{defaultResolution.height}");
        fullscreenToggle.isOn = true;
        InitializeVolumes();
    }
}
