using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingsMenu : MonoBehaviour
{
    [Space(3), Header("Settings Objects")]
    [SerializeField] private GameObject _Fullscreen;
    [SerializeField] private GameObject _Resolution;
    [SerializeField] private GameObject _VerticalSync;
    [SerializeField] private GameObject _MasterVolume;
    [SerializeField] private GameObject _MusicVolume;
    [SerializeField] private GameObject _EffectsVolume;
    [SerializeField] private GameObject _AmbientVolume;

    [Space(3), Header("Other")]
    [SerializeField] private Button _ApplyButton = null;

    private Toggle _FullscreenToggle = null;
    private TMP_Dropdown _ResolutionDropdown = null;
    private TMP_Dropdown _VSyncDropdown = null;
    private Slider _MasterVolumeSlider = null;
    private Slider _MusicVolumeSlider = null;
    private Slider _EffectsVolumeSlider = null;
    private Slider _AmbientVolumeSlider = null;
    private TMP_Text _MasterVolumeText = null;
    private TMP_Text _MusicVolumeText = null;
    private TMP_Text _EffectsVolumeText = null;
    private TMP_Text _AmbientVolumeText = null;

    private SettingsManager m_SettingsManager = null;

    private void Awake()
    {
        m_SettingsManager = GetComponent<SettingsManager>();

        _FullscreenToggle = _Fullscreen.GetComponentInChildren<Toggle>();

        _ResolutionDropdown = _Resolution.GetComponentInChildren<TMP_Dropdown>();
        _VSyncDropdown = _VerticalSync.GetComponentInChildren<TMP_Dropdown>();

        _MasterVolumeSlider = _MasterVolume.GetComponentInChildren<Slider>();
        _MusicVolumeSlider = _MusicVolume.GetComponentInChildren<Slider>();
        _EffectsVolumeSlider = _EffectsVolume.GetComponentInChildren<Slider>();
        _AmbientVolumeSlider = _AmbientVolume.GetComponentInChildren<Slider>();

        _MasterVolumeText = _MasterVolume.GetComponentInChildren<TMP_Text>();
        _MusicVolumeText = _MusicVolume.GetComponentInChildren<TMP_Text>();
        _EffectsVolumeText = _EffectsVolume.GetComponentInChildren<TMP_Text>();
        _AmbientVolumeText = _AmbientVolume.GetComponentInChildren<TMP_Text>();

        _MasterVolumeSlider.onValueChanged.AddListener(delegate { OnMasterVolumeChange(); });
        _MusicVolumeSlider.onValueChanged.AddListener(delegate { OnMusicVolumeChange(); });
        _EffectsVolumeSlider.onValueChanged.AddListener(delegate { OnEffectsVolumeChange(); });
        _AmbientVolumeSlider.onValueChanged.AddListener(delegate { OnAmbientVolumeChange(); });

        _ApplyButton.onClick.AddListener(delegate { ApplyChanges(); });
    }

    private void OnMasterVolumeChange()
    {
        _MasterVolumeText.text = "Master Volume = " + (int)(_MasterVolumeSlider.value * 100) + "%";
    }
    private void OnMusicVolumeChange()
    {
        _MusicVolumeText.text = "Music Volume = " + (int)(_MusicVolumeSlider.value * 100) + "%";
    }
    private void OnEffectsVolumeChange()
    {
        _EffectsVolumeText.text = "Sound Effects Volume = " + (int)(_EffectsVolumeSlider.value * 100) + "%";
    }
    private void OnAmbientVolumeChange()
    {
        _AmbientVolumeText.text = "Ambient Volume = " + (int)(_AmbientVolumeSlider.value * 100) + "%";
    }

    private void ApplyChanges()
    {
        GameSettings gs = SaveToGameSettings();

        m_SettingsManager.Save(ref gs);
        m_SettingsManager.Apply();
    }

    /// <summary>
    /// Get current selected settings from UI and save to GameSettings
    /// </summary>
    private GameSettings SaveToGameSettings()
    {
        GameSettings currentSettings = new GameSettings();

        currentSettings.IsFullscreen = _FullscreenToggle.isOn;

        currentSettings.ResolutionIndex = _ResolutionDropdown.value;
        currentSettings.VerticalSync = _VSyncDropdown.value;

        currentSettings.MasterVolume = _MasterVolumeSlider.value;
        currentSettings.MusicVolume = _MusicVolumeSlider.value;
        currentSettings.EffectsVolume = _EffectsVolumeSlider.value;
        currentSettings.AmbientVolume = _AmbientVolumeSlider.value;

        return currentSettings;
    }

    /// <summary>
    /// Use GameSettings to assign a value to each corresponding UI element
    /// </summary>
    public void Refresh(ref GameSettings gameSettings)
    {
        _FullscreenToggle.isOn = gameSettings.IsFullscreen;

        _ResolutionDropdown.value = gameSettings.ResolutionIndex;
        _VSyncDropdown.value = gameSettings.VerticalSync;

        if (_ResolutionDropdown.options.Count == 0)
        {
            foreach (Resolution resolution in m_SettingsManager.Resolutions)
            {
                _ResolutionDropdown.options.Add(new TMP_Dropdown.OptionData(resolution.ToString()));
            }
        }
        _ResolutionDropdown.value = gameSettings.ResolutionIndex;

        _ResolutionDropdown.RefreshShownValue();

        _MasterVolumeSlider.value = gameSettings.MasterVolume;
        _MasterVolumeText.text = "Master Volume = " + (int)(_MasterVolumeSlider.value * 100) + "%";

        _MusicVolumeSlider.value = gameSettings.MusicVolume;
        _MusicVolumeText.text = "Music Volume = " + (int)(_MusicVolumeSlider.value * 100) + "%";

        _EffectsVolumeSlider.value = gameSettings.EffectsVolume;
        _EffectsVolumeText.text = "Sound Effects Volume = " + (int)(_EffectsVolumeSlider.value * 100) + "%";

        _AmbientVolumeSlider.value = gameSettings.AmbientVolume;
        _AmbientVolumeText.text = "Ambient Volume = " + (int)(_AmbientVolumeSlider.value * 100) + "%";
    }
}
