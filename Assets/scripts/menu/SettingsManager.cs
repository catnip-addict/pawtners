using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public enum InputDeviceType
{
    Mouse,
    GamepadKeyboard
}
public class SettingsManager : MonoBehaviour
{
    [Header("Panele Menu")]
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject customizationPanel;
    [SerializeField] private GameObject achievementPanel;
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject creditsPanel;

    [Header("Kamery")]
    [SerializeField] private GameObject mainMenuCamera;
    [SerializeField] private GameObject customizationCamera;
    [SerializeField] private GameObject achievementCamera;
    [SerializeField] private GameObject settingsCamera;
    [SerializeField] private GameObject creditsCamera;

    [Header("Pierwsze guziki")]
    [SerializeField] private GameObject mainMenuFirstButton;
    [SerializeField] private GameObject customizationFirstButton;
    [SerializeField] private GameObject achievementFirstButton;
    [SerializeField] private GameObject settingsMenuFirstButton;
    [SerializeField] private GameObject creditsPanelFirstButton;

    [Header("Ustawienia Dźwięku")]
    [SerializeField] private Slider masterVolumeSlider;
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider sfxVolumeSlider;

    [Header("Ustawienia Grafiki")]
    [SerializeField] private TMP_Dropdown qualityDropdown;
    [SerializeField] private Toggle fullscreenToggle;
    [SerializeField] private TMP_Dropdown resolutionDropdown;

    [Header("Ustawienia Sterowania")]
    // [SerializeField] private bool enableControllerSupport = true;
    [SerializeField] private Slider mouseSensSlider;
    // [SerializeField] private float controllerSensitivity = 0.5f;
    public InputDeviceType currentInputDevice;

    private Resolution[] resolutions;

    private void Start()
    {
        if (mainMenuPanel != null)
        {
            // Debug.LogError("Main Menu Panel is not assigned in the inspector.");
            ShowMainMenu();
        }
        InitializeResolutions();
        InitializeQualityDropdown();
        LoadSettings();
    }
    void Update()
    {
        if (Input.anyKeyDown)
        {
            if (currentInputDevice != InputDeviceType.GamepadKeyboard)
            {
                currentInputDevice = InputDeviceType.GamepadKeyboard;
                ChangeInputDevice();
            }
        }
        if (Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0)
        {
            currentInputDevice = InputDeviceType.Mouse;
            EventSystem.current.SetSelectedGameObject(null);
        }
        for (int i = 0; i < 20; i++)
        {
            if (Input.GetKeyDown(KeyCode.Joystick1Button0 + i))
            {
                if (currentInputDevice != InputDeviceType.GamepadKeyboard)
                {
                    currentInputDevice = InputDeviceType.GamepadKeyboard;
                    ChangeInputDevice();
                }
                break;
            }
        }

        // Check for joystick axes movement
        if (Mathf.Abs(Input.GetAxis("Horizontal")) > 0.2f ||
            Mathf.Abs(Input.GetAxis("Vertical")) > 0.2f)
        {
            if (currentInputDevice != InputDeviceType.GamepadKeyboard)
            {
                currentInputDevice = InputDeviceType.GamepadKeyboard;
                ChangeInputDevice();
            }
        }
    }
    void ChangeInputDevice()
    {
        if (currentInputDevice == InputDeviceType.GamepadKeyboard)
        {
            if (mainMenuPanel.activeSelf)
            {
                EventSystem.current.SetSelectedGameObject(mainMenuFirstButton);
            }
            if (settingsPanel.activeSelf)
            {
                EventSystem.current.SetSelectedGameObject(settingsMenuFirstButton);
            }
            if (creditsPanel.activeSelf)
            {
                EventSystem.current.SetSelectedGameObject(creditsPanelFirstButton);
            }
            if (customizationPanel.activeSelf)
            {
                EventSystem.current.SetSelectedGameObject(customizationFirstButton);
            }
        }
    }
    private void LoadSettings()
    {
        float masterVolume = PlayerPrefs.GetFloat("MasterVolume", 1.0f);
        float musicVolume = PlayerPrefs.GetFloat("MusicVolume", 1.0f);
        float sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 1.0f);
        float mouseSens = PlayerPrefs.GetFloat("MouseSens", 1.0f);

        masterVolumeSlider.value = masterVolume;
        musicVolumeSlider.value = musicVolume;
        sfxVolumeSlider.value = sfxVolume;
        mouseSensSlider.value = mouseSens;

        fullscreenToggle.isOn = PlayerPrefs.GetInt("Fullscreen", Screen.fullScreen ? 1 : 0) == 1;

        SetMasterVolume(masterVolume);
        SetMusicVolume(musicVolume);
        SetSFXVolume(sfxVolume);
        SetMouseSens(mouseSens);
        SetFullscreen(fullscreenToggle.isOn);
        SetResolution(resolutionDropdown.value);
    }
    private void InitializeQualityDropdown()
    {
        qualityDropdown.ClearOptions();

        System.Collections.Generic.List<string> qualityOptions = new System.Collections.Generic.List<string>();
        for (int i = 0; i < QualitySettings.names.Length; i++)
        {
            qualityOptions.Add(QualitySettings.names[i]);
        }

        qualityDropdown.AddOptions(qualityOptions);

        int savedQualityLevel = PlayerPrefs.GetInt("QualityLevel", QualitySettings.GetQualityLevel());
        qualityDropdown.value = savedQualityLevel;
        qualityDropdown.RefreshShownValue();
    }
    private void InitializeResolutions()
    {
        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();

        int currentResolutionIndex = 0;
        System.Collections.Generic.List<string> options = new System.Collections.Generic.List<string>();

        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            options.Add(option);

            if (resolutions[i].width == Screen.currentResolution.width &&
                resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = PlayerPrefs.GetInt("ResolutionIndex", currentResolutionIndex);
        resolutionDropdown.RefreshShownValue();
    }
    public void ShowMainMenu()
    {
        mainMenuPanel.SetActive(true);
        settingsPanel.SetActive(false);
        creditsPanel.SetActive(false);
        customizationPanel.SetActive(false);
        achievementPanel.SetActive(false);
        /// Camera
        mainMenuCamera.SetActive(true);
        settingsCamera.SetActive(false);
        creditsCamera.SetActive(false);
        customizationCamera.SetActive(false);
        achievementCamera.SetActive(false);
        ChangeInputDevice();
    }

    public void ShowSettings()
    {
        mainMenuPanel.SetActive(false);
        settingsPanel.SetActive(true);
        creditsPanel.SetActive(false);
        customizationPanel.SetActive(false);
        achievementPanel.SetActive(false);
        /// Camera
        mainMenuCamera.SetActive(false);
        settingsCamera.SetActive(true);
        creditsCamera.SetActive(false);
        customizationCamera.SetActive(false);
        achievementCamera.SetActive(false);
        ChangeInputDevice();
    }

    public void ShowCredits()
    {
        mainMenuPanel.SetActive(false);
        settingsPanel.SetActive(false);
        creditsPanel.SetActive(true);
        customizationPanel.SetActive(false);
        achievementPanel.SetActive(false);
        /// Camera
        mainMenuCamera.SetActive(false);
        settingsCamera.SetActive(false);
        creditsCamera.SetActive(true);
        customizationCamera.SetActive(false);
        achievementCamera.SetActive(false);
        ChangeInputDevice();
    }
    public void ShowCustomization()
    {
        mainMenuPanel.SetActive(false);
        settingsPanel.SetActive(false);
        creditsPanel.SetActive(false);
        customizationPanel.SetActive(true);
        achievementPanel.SetActive(false);
        /// Camera
        mainMenuCamera.SetActive(false);
        settingsCamera.SetActive(false);
        creditsCamera.SetActive(false);
        customizationCamera.SetActive(true);
        achievementCamera.SetActive(false);
        ChangeInputDevice();
    }
    public void ShowAchievements()
    {
        mainMenuPanel.SetActive(false);
        settingsPanel.SetActive(false);
        creditsPanel.SetActive(false);
        customizationPanel.SetActive(false);
        achievementPanel.SetActive(true);
        /// Camera
        mainMenuCamera.SetActive(false);
        settingsCamera.SetActive(false);
        creditsCamera.SetActive(false);
        customizationCamera.SetActive(false);
        achievementCamera.SetActive(true);
        ChangeInputDevice();
    }
    public void StartGame()
    {
        StartCoroutine(TransitionManager.Instance.TransitionToScene(1));
    }

    public void QuitGame()
    {
        SaveSettings();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }

    public void SetMasterVolume(float volume)
    {
        AudioListener.volume = volume;
        PlayerPrefs.SetFloat("MasterVolume", volume);
    }

    public void SetMusicVolume(float volume)
    {
        PlayerPrefs.SetFloat("MusicVolume", volume);
        SoundManager.Instance.UpdateMusicVolume(volume);
    }

    public void SetSFXVolume(float volume)
    {
        PlayerPrefs.SetFloat("SFXVolume", volume);
        SoundManager.Instance.UpdateSFXVolume(volume);
    }

    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
        PlayerPrefs.SetInt("QualityLevel", qualityIndex);
        PlayerPrefs.Save();
    }

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
        PlayerPrefs.SetInt("Fullscreen", isFullscreen ? 1 : 0);
    }

    public void SetResolution(int resolutionIndex)
    {
        if (resolutions == null || resolutions.Length == 0)
        {
            Debug.LogWarning("No resolutions available to set");
            return;
        }

        if (resolutionIndex < 0 || resolutionIndex >= resolutions.Length)
        {
            Debug.LogWarning("Resolution index out of range: " + resolutionIndex);
            resolutionIndex = 0;
        }

        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height,
            Screen.fullScreen ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed,
            resolution.refreshRateRatio);
        PlayerPrefs.SetInt("ResolutionIndex", resolutionIndex);
    }
    public void SetMouseSens(float mouseSens)
    {
        PlayerPrefs.SetFloat("MouseSens", mouseSens);
    }
    public void SaveSettings()
    {
        PlayerPrefs.Save();
    }
}