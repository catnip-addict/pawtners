using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
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
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject creditsPanel;
    [Header("Pierwsze guziki")]
    [SerializeField] private GameObject mainMenuFirstButton;
    [SerializeField] private GameObject settingsMenuFirstButton;
    [SerializeField] private GameObject creditsPanelFirstButton;

    [Header("Ustawienia Dźwięku")]
    [SerializeField] private Slider masterVolumeSlider;
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider sfxVolumeSlider;
    [SerializeField] private TextMeshProUGUI masterVolumeText;
    [SerializeField] private TextMeshProUGUI musicVolumeText;
    [SerializeField] private TextMeshProUGUI sfxVolumeText;

    [Header("Ustawienia Grafiki")]
    [SerializeField] private TMP_Dropdown qualityDropdown;
    [SerializeField] private Toggle fullscreenToggle;
    [SerializeField] private TMP_Dropdown resolutionDropdown;

    [Header("Ustawienia Sterowania")]
    [SerializeField] private bool enableControllerSupport = true;
    [SerializeField] private float controllerSensitivity = 0.5f;
    public InputDeviceType currentInputDevice;

    private Resolution[] resolutions;

    private void Start()
    {
        ShowMainMenu();
        LoadSettings();
        InitializeResolutions();
        // SetupControllerNavigation();
    }
    void Update()
    {
        if (Input.anyKeyDown)
        {
            ChangeInputDevice();
            currentInputDevice = InputDeviceType.GamepadKeyboard;
        }
        if (Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0)
        {
            currentInputDevice = InputDeviceType.Mouse;
            EventSystem.current.SetSelectedGameObject(null);
        }

    }
    void ChangeInputDevice()
    {
        if (currentInputDevice == InputDeviceType.GamepadKeyboard)
            return;
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
    }
    private void LoadSettings()
    {
        float masterVolume = PlayerPrefs.GetFloat("MasterVolume", 1.0f);
        float musicVolume = PlayerPrefs.GetFloat("MusicVolume", 1.0f);
        float sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 1.0f);

        masterVolumeSlider.value = masterVolume;
        musicVolumeSlider.value = musicVolume;
        sfxVolumeSlider.value = sfxVolume;

        if (masterVolumeText != null)
            masterVolumeText.text = $"{Mathf.RoundToInt(masterVolume * 100)}%";
        if (musicVolumeText != null)
            musicVolumeText.text = $"{Mathf.RoundToInt(musicVolume * 100)}%";
        if (sfxVolumeText != null)
            sfxVolumeText.text = $"{Mathf.RoundToInt(sfxVolume * 100)}%";

        qualityDropdown.value = PlayerPrefs.GetInt("QualityLevel", QualitySettings.GetQualityLevel());
        fullscreenToggle.isOn = PlayerPrefs.GetInt("Fullscreen", Screen.fullScreen ? 1 : 0) == 1;
    }

    private void InitializeResolutions()
    {
        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();

        int currentResolutionIndex = 0;
        System.Collections.Generic.List<string> options = new System.Collections.Generic.List<string>();

        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height + " @ " + resolutions[i].refreshRateRatio + "Hz";
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

    public void SetupControllerNavigation()
    {
        if (enableControllerSupport)
        {
            Button[] allButtons = FindObjectsByType<Button>(FindObjectsSortMode.None);
            foreach (Button button in allButtons)
            {
                Navigation navigation = button.navigation;
                navigation.mode = Navigation.Mode.Automatic;
                button.navigation = navigation;
            }

            Slider[] allSliders = FindObjectsByType<Slider>(FindObjectsSortMode.None);
            foreach (Slider slider in allSliders)
            {
                Navigation navigation = slider.navigation;
                navigation.mode = Navigation.Mode.Automatic;
                slider.navigation = navigation;
            }

            Toggle[] allToggles = FindObjectsByType<Toggle>(FindObjectsSortMode.None);
            foreach (Toggle toggle in allToggles)
            {
                Navigation navigation = toggle.navigation;
                navigation.mode = Navigation.Mode.Automatic;
                toggle.navigation = navigation;
            }

            TMP_Dropdown[] allDropdowns = FindObjectsByType<TMP_Dropdown>(FindObjectsSortMode.None);
            foreach (TMP_Dropdown dropdown in allDropdowns)
            {
                Navigation navigation = dropdown.navigation;
                navigation.mode = Navigation.Mode.Automatic;
                dropdown.navigation = navigation;
            }
        }
    }

    public void ShowMainMenu()
    {
        mainMenuPanel.SetActive(true);
        settingsPanel.SetActive(false);
        creditsPanel.SetActive(false);
        EventSystem.current.SetSelectedGameObject(mainMenuFirstButton);
    }

    public void ShowSettings()
    {
        mainMenuPanel.SetActive(false);
        settingsPanel.SetActive(true);
        creditsPanel.SetActive(false);
        EventSystem.current.SetSelectedGameObject(settingsMenuFirstButton);
    }

    public void ShowCredits()
    {
        mainMenuPanel.SetActive(false);
        settingsPanel.SetActive(false);
        creditsPanel.SetActive(true);
        // EventSystem.current.SetSelectedGameObject(creditsPanelFirstButton);
    }

    public void StartGame()
    {
        SceneManager.LoadScene("GameScene");
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

        if (masterVolumeText != null)
            masterVolumeText.text = $"{Mathf.RoundToInt(volume * 100)}%";
    }

    public void SetMusicVolume(float volume)
    {
        PlayerPrefs.SetFloat("MusicVolume", volume);

        if (musicVolumeText != null)
            musicVolumeText.text = $"{Mathf.RoundToInt(volume * 100)}%";

        AudioManager audioManager = FindFirstObjectByType<AudioManager>();
        if (audioManager != null)
        {
            audioManager.UpdateMusicVolume(volume);
        }
    }

    public void SetSFXVolume(float volume)
    {
        PlayerPrefs.SetFloat("SFXVolume", volume);

        if (sfxVolumeText != null)
            sfxVolumeText.text = $"{Mathf.RoundToInt(volume * 100)}%";

        AudioManager audioManager = FindFirstObjectByType<AudioManager>();
        if (audioManager != null)
        {
            audioManager.UpdateSFXVolume(volume);
        }
    }

    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
        PlayerPrefs.SetInt("QualityLevel", qualityIndex);
    }

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
        PlayerPrefs.SetInt("Fullscreen", isFullscreen ? 1 : 0);
    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height,
            Screen.fullScreen ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed,
            resolution.refreshRateRatio);
        PlayerPrefs.SetInt("ResolutionIndex", resolutionIndex);
    }

    public void SaveSettings()
    {
        PlayerPrefs.Save();
    }
}