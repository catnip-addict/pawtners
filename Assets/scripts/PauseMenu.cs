using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;
using TMPro;


public class PauseMenu : MonoBehaviour
{

    public static PauseMenu Instance { get; private set; }
    [Header("Panele Menu")]
    [SerializeField] private GameObject pauseMenuUI;
    [SerializeField] private GameObject settingsMenuUI;
    [Header("Ustawienia Dźwięku")]
    [SerializeField] private Slider masterVolumeSlider;
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider sfxVolumeSlider;

    [Header("Ustawienia Grafiki")]
    [SerializeField] private TMP_Dropdown qualityDropdown;
    [SerializeField] private Toggle fullscreenToggle;
    [SerializeField] private TMP_Dropdown resolutionDropdown;

    [Header("Ustawienia Sterowania")]
    [SerializeField] private Slider mouseSensSlider;
    [SerializeField] private Slider joystickSensSlider;
    public InputDeviceType currentInputDevice;
    public bool isBusy = false;

    private Resolution[] resolutions;
    bool gameIsPaused = false;
    [SerializeField] private GameObject mainMenuFirstButton;
    [SerializeField] private GameObject settingsMenuFirstButton;

    private void Awake()
    {
        // If there is an instance, and it's not me, I should kill myself.

        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }
    void Start()
    {
        InitializeQualityDropdown();
        InitializeResolutions();
        LoadSettings();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
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
            if (pauseMenuUI.activeSelf)
            {
                EventSystem.current.SetSelectedGameObject(mainMenuFirstButton);
            }
            if (settingsMenuUI.activeSelf)
            {
                EventSystem.current.SetSelectedGameObject(settingsMenuFirstButton);
            }
        }
    }
    public void CheckForPause()
    {
        if (gameIsPaused && settingsMenuUI.activeSelf)
        {
            BackToPauseMenu();
        }
        else if (gameIsPaused && !settingsMenuUI.activeSelf)
        {
            ResumeGame();
        }
        else
        {
            if (!isBusy)
            {
                PauseGame();
            }

        }
    }
    public void OpenSettingsMenu()
    {
        settingsMenuUI.SetActive(true);
        pauseMenuUI.SetActive(false);
    }
    public void BackToPauseMenu()
    {
        settingsMenuUI.SetActive(false);
        pauseMenuUI.SetActive(true);
    }

    public void ResumeGame()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1.0f;
        gameIsPaused = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void PauseGame()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        gameIsPaused = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    public void LoadMenu()
    {
        SceneManager.LoadScene(0);
        Time.timeScale = 1.0f;
    }
    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }
    private void LoadSettings()
    {
        // Load audio settings
        float masterVolume = PlayerPrefs.GetFloat("MasterVolume", 1.0f);
        float musicVolume = PlayerPrefs.GetFloat("MusicVolume", 1.0f);
        float sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 1.0f);
        float mouseSens = PlayerPrefs.GetFloat("MouseSens", 1.0f);
        float joystickSens = PlayerPrefs.GetFloat("JoystickSens", 1.0f);

        // Apply loaded values to UI if references exist
        if (masterVolumeSlider) masterVolumeSlider.value = masterVolume;
        if (musicVolumeSlider) musicVolumeSlider.value = musicVolume;
        if (sfxVolumeSlider) sfxVolumeSlider.value = sfxVolume;
        if (mouseSensSlider) mouseSensSlider.value = mouseSens;
        if (joystickSensSlider) joystickSensSlider.value = joystickSens;

        if (fullscreenToggle)
            fullscreenToggle.isOn = PlayerPrefs.GetInt("Fullscreen", Screen.fullScreen ? 1 : 0) == 1;

        // Apply settings
        SetMasterVolume(masterVolume);
        SetMusicVolume(musicVolume);
        SetSFXVolume(sfxVolume);
        SetMouseSens(mouseSens);
        SetJoystickSens(joystickSens);
        SetFullscreen(fullscreenToggle ? fullscreenToggle.isOn : Screen.fullScreen);

#if !UNITY_EDITOR
        if (resolutionDropdown && resolutions != null && resolutions.Length > 0)
            SetResolution(resolutionDropdown.value);
#endif
    }

    public void InitializeQualityDropdown()
    {
        if (qualityDropdown == null) return;

        qualityDropdown.ClearOptions();

        List<string> qualityOptions = new List<string>();
        for (int i = 0; i < QualitySettings.names.Length; i++)
        {
            qualityOptions.Add(QualitySettings.names[i]);
        }

        qualityDropdown.AddOptions(qualityOptions);

        int savedQualityLevel = PlayerPrefs.GetInt("QualityLevel", QualitySettings.GetQualityLevel());
        qualityDropdown.value = savedQualityLevel;
        qualityDropdown.RefreshShownValue();
    }

    public void InitializeResolutions()
    {
        if (resolutionDropdown == null) return;

        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();

        int currentResolutionIndex = 0;
        List<string> options = new List<string>();

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

    public void SetMasterVolume(float volume)
    {
        AudioListener.volume = volume;
        PlayerPrefs.SetFloat("MasterVolume", volume);
    }

    public void SetMusicVolume(float volume)
    {
        PlayerPrefs.SetFloat("MusicVolume", volume);
        SoundManager audioManager = FindAnyObjectByType<SoundManager>();
        if (audioManager != null)
        {
            audioManager.UpdateMusicVolume(volume);
        }
    }

    public void SetSFXVolume(float volume)
    {
        PlayerPrefs.SetFloat("SFXVolume", volume);
        SoundManager audioManager = FindAnyObjectByType<SoundManager>();
        if (audioManager != null)
        {
            audioManager.UpdateSFXVolume(volume);
        }
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
        if (resolutions == null || resolutionIndex >= resolutions.Length) return;

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
    public void SetJoystickSens(float joystickSens)
    {
        PlayerPrefs.SetFloat("JoystickSens", joystickSens);
    }
    public void UpdateInputDevice(InputDeviceType newDeviceType)
    {
        currentInputDevice = newDeviceType;
    }

    public float GetMouseSensitivity()
    {
        return PlayerPrefs.GetFloat("MouseSens", 1.0f);
    }

    public void SaveSettings()
    {
        Player player1 = GameManager.Instance.player1;
        Player player2 = GameManager.Instance.player2;
        player1.SetSensitivity();
        player2.SetSensitivity();
        PlayerPrefs.Save();
    }
    void Tp(int id)
    {
        StartCoroutine(TransitionManager.Instance.TransitionToScene(id));
    }
    public void Resetlevel()
    {
        Time.timeScale = 1.0f;
        Tp(SceneManager.GetActiveScene().buildIndex);
    }
}
