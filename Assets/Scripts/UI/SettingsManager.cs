using System;
using UnityEngine;
using UnityEngine.Audio;

public struct GameSettings
{
    public bool IsFullscreen;
    public int ResolutionIndex;
    public int VerticalSync;
    public float MasterVolume;
    public float MusicVolume;
    public float EffectsVolume;
    public float AmbientVolume;
}

public class SettingsManager : MonoBehaviour
{
    [SerializeField] 
    private AudioMixer _MainMixer = null;

    private SettingsMenu _SettingsMenu = null;

    private GameSettings _GameSettings;
    private Resolution[] _Resolutions;

    private const string IsFullscreen = "IsFullscreen";
    private const string ResolutionIndex = "ResolutionIndex";
    private const string VerticalSync = "VerticalSync";
    private const string MasterVolume = "MasterVolume";
    private const string MusicVolume = "MusicVolume";
    private const string EffectsVolume = "EffectsVolume";
    private const string AmbientVolume = "AmbientVolume";

    public Resolution[] Resolutions => _Resolutions;

    private void Start()
    {
        _SettingsMenu = GetComponent<SettingsMenu>();

        _GameSettings = new GameSettings();
        _Resolutions = Screen.resolutions;

        Load();
        Apply();

        _SettingsMenu.Refresh(ref _GameSettings);
    }

    /// <summary>
    /// Get saved data from PlayerPrefs and assign each corresponding value in GameSettings
    /// </summary>
    private void Load()
    {
        _GameSettings.IsFullscreen = Convert.ToBoolean(PlayerPrefs.GetInt(IsFullscreen, 0));
        _GameSettings.ResolutionIndex = PlayerPrefs.GetInt(ResolutionIndex, _Resolutions.Length - 1);

        _GameSettings.VerticalSync = PlayerPrefs.GetInt(VerticalSync, QualitySettings.vSyncCount);

        _GameSettings.MasterVolume = PlayerPrefs.GetFloat(MasterVolume, 0.75f);
        _GameSettings.MusicVolume = PlayerPrefs.GetFloat(MusicVolume, 1.0f);
        _GameSettings.EffectsVolume = PlayerPrefs.GetFloat(EffectsVolume, 1.0f);
        _GameSettings.AmbientVolume = PlayerPrefs.GetFloat(AmbientVolume, 1.0f);

        _GameSettings.ResolutionIndex = (_GameSettings.ResolutionIndex >= _Resolutions.Length) 
            ? _Resolutions.Length - 1 : _GameSettings.ResolutionIndex;
    }

    /// <summary>
    /// Update game's settings using each corresponding value from GameSettings
    /// </summary>
    public void Apply()
    {
        Screen.fullScreen = _GameSettings.IsFullscreen;
        Screen.SetResolution(
            _Resolutions[_GameSettings.ResolutionIndex].width,
            _Resolutions[_GameSettings.ResolutionIndex].height, _GameSettings.IsFullscreen);

        QualitySettings.vSyncCount = _GameSettings.VerticalSync;

        if (_MainMixer != null)
        {
            _MainMixer.SetFloat("MasterVolume", Mathf.Log(_GameSettings.MasterVolume) * 20);
            _MainMixer.SetFloat("MusicVolume", Mathf.Log(_GameSettings.MusicVolume) * 20);
            _MainMixer.SetFloat("EffectsVolume", Mathf.Log(_GameSettings.EffectsVolume) * 20);
            _MainMixer.SetFloat("AmbientVolume", Mathf.Log(_GameSettings.AmbientVolume) * 20);
        }
    }

    /// <summary>
    /// Save GameSettings to file using PlayerPrefs
    /// </summary>
    public void Save(ref GameSettings gameSettings)
    {
        _GameSettings = gameSettings;

        PlayerPrefs.SetInt(IsFullscreen, _GameSettings.IsFullscreen ? 1 : 0);
        PlayerPrefs.SetInt(VerticalSync, _GameSettings.VerticalSync);
        PlayerPrefs.SetInt(ResolutionIndex, _GameSettings.ResolutionIndex);
        PlayerPrefs.SetFloat(MasterVolume, _GameSettings.MasterVolume);
        PlayerPrefs.SetFloat(MusicVolume, _GameSettings.MusicVolume);
        PlayerPrefs.SetFloat(EffectsVolume, _GameSettings.EffectsVolume);
        PlayerPrefs.SetFloat(AmbientVolume, _GameSettings.AmbientVolume);
    }
}
