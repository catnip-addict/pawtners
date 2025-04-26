using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class MenuInputController : MonoBehaviour
{
    [Header("Ustawienia Nawigacji")]
    [SerializeField] private float navigationCooldown = 0.2f;
    [SerializeField] private float horizontalSensitivity = 0.5f;
    [SerializeField] private float verticalSensitivity = 0.5f;
    [SerializeField] private Button firstSelectedButton;

    [Header("Komponenty UI")]
    [SerializeField] private Selectable[] menuSelectable;
    [SerializeField] private Selectable[] settingsSelectable;
    [SerializeField] private Selectable[] creditsSelectable;

    private bool isNavigating = false;
    private float lastMoveTime = 0f;
    private GameObject currentPanel;

    private void Start()
    {
        // Ustaw domyślnie wybrany element
        if (firstSelectedButton != null)
        {
            EventSystem.current.SetSelectedGameObject(firstSelectedButton.gameObject);
        }

        // Zapisz aktualny panel
        currentPanel = GameObject.FindGameObjectWithTag("MainMenuPanel");
    }

    private void Update()
    {
        // Sprawdź czy aktualnie jest wybrany element
        if (EventSystem.current.currentSelectedGameObject == null)
        {
            // Jeśli nie ma wybranego elementu, wybierz domyślny
            RestoreSelection();
        }

        // Obsługa przycisku powrotu/wstecz (np. B na Xboxie, Circle na PlayStation)
        if (Input.GetButtonDown("Cancel") || Input.GetKeyDown(KeyCode.Escape))
        {
            HandleBackButton();
        }

        // Obsługa przycisku potwierdzenia (np. A na Xboxie, X na PlayStation)
        if (Input.GetButtonDown("Submit") || Input.GetKeyDown(KeyCode.Return))
        {
            HandleSubmitButton();
        }

        // Obsługa suwaka za pomocą przycisków kierunkowych lub analogowych
        HandleSliderNavigation();
    }

    private void RestoreSelection()
    {
        // Określ, który panel jest aktywny
        GameObject mainMenuPanel = GameObject.FindGameObjectWithTag("MainMenuPanel");
        GameObject settingsPanel = GameObject.FindGameObjectWithTag("SettingsPanel");
        GameObject creditsPanel = GameObject.FindGameObjectWithTag("CreditsPanel");

        // Wybierz odpowiedni pierwszy element w zależności od aktywnego panelu
        if (mainMenuPanel != null && mainMenuPanel.activeSelf)
        {
            EventSystem.current.SetSelectedGameObject(menuSelectable.Length > 0 ? menuSelectable[0].gameObject : firstSelectedButton.gameObject);
            currentPanel = mainMenuPanel;
        }
        else if (settingsPanel != null && settingsPanel.activeSelf)
        {
            EventSystem.current.SetSelectedGameObject(settingsSelectable.Length > 0 ? settingsSelectable[0].gameObject : null);
            currentPanel = settingsPanel;
        }
        else if (creditsPanel != null && creditsPanel.activeSelf)
        {
            EventSystem.current.SetSelectedGameObject(creditsSelectable.Length > 0 ? creditsSelectable[0].gameObject : null);
            currentPanel = creditsPanel;
        }
    }

    private void HandleBackButton()
    {
        // Zachowanie przycisku powrotu w zależności od aktywnego panelu
        GameObject mainMenuPanel = GameObject.FindGameObjectWithTag("MainMenuPanel");
        GameObject settingsPanel = GameObject.FindGameObjectWithTag("SettingsPanel");
        GameObject creditsPanel = GameObject.FindGameObjectWithTag("CreditsPanel");

        // Powrót do menu głównego z innych paneli
        if ((settingsPanel != null && settingsPanel.activeSelf) ||
            (creditsPanel != null && creditsPanel.activeSelf))
        {
            // Wywołaj funkcję ShowMainMenu z MainMenuManager
            MainMenuManager menuManager = FindObjectOfType<MainMenuManager>();
            if (menuManager != null)
            {
                menuManager.ShowMainMenu();
                // Ustaw wybrany element w menu głównym
                if (menuSelectable.Length > 0)
                {
                    EventSystem.current.SetSelectedGameObject(menuSelectable[0].gameObject);
                }
            }
        }
        else if (mainMenuPanel != null && mainMenuPanel.activeSelf)
        {
            // Możemy np. otworzyć panel potwierdzenia wyjścia z gry
            // W tym przykładzie po prostu wywołujemy QuitGame
            MainMenuManager menuManager = FindObjectOfType<MainMenuManager>();
            if (menuManager != null)
            {
                menuManager.QuitGame();
            }
        }
    }

    private void HandleSubmitButton()
    {
        // Obsługa przycisku potwierdzenia jest automatycznie obsługiwana przez system UI Unity
        // Ten kod jest tutaj jako szkielet, gdybyś chciał dodać dodatkową logikę
        GameObject currentSelection = EventSystem.current.currentSelectedGameObject;

        if (currentSelection != null)
        {
            // Możesz dodać tutaj dodatkową logikę, np. efekty dźwiękowe
            AudioManager audioManager = FindObjectOfType<AudioManager>();
            if (audioManager != null)
            {
                audioManager.PlayButtonClick();
            }
        }
    }

    private void HandleSliderNavigation()
    {
        GameObject currentSelection = EventSystem.current.currentSelectedGameObject;

        if (currentSelection != null)
        {
            Slider slider = currentSelection.GetComponent<Slider>();
            if (slider != null)
            {
                float horizontalInput = Input.GetAxis("Horizontal");

                // Unikaj ciągłej zmiany przez dodanie opóźnienia
                if (Mathf.Abs(horizontalInput) > horizontalSensitivity && Time.time > lastMoveTime + navigationCooldown)
                {
                    lastMoveTime = Time.time;

                    // Dostosuj wartość suwaka 
                    float increment = 0.1f * Mathf.Sign(horizontalInput);
                    slider.value = Mathf.Clamp(slider.value + increment, slider.minValue, slider.maxValue);

                    // Wywołaj zdarzenie onValueChanged
                    slider.onValueChanged.Invoke(slider.value);
                }
            }

            // Obsługa przełączników (Toggle)
            Toggle toggle = currentSelection.GetComponent<Toggle>();
            if (toggle != null)
            {
                float horizontalInput = Input.GetAxis("Horizontal");

                if (Mathf.Abs(horizontalInput) > horizontalSensitivity && Time.time > lastMoveTime + navigationCooldown)
                {
                    lastMoveTime = Time.time;

                    // Przełącz stan dla Toggle
                    toggle.isOn = !toggle.isOn;

                    // Wywołaj zdarzenie onValueChanged
                    toggle.onValueChanged.Invoke(toggle.isOn);
                }
            }

            // Obsługa dropdown
            Dropdown dropdown = currentSelection.GetComponent<Dropdown>();
            if (dropdown != null)
            {
                float horizontalInput = Input.GetAxis("Horizontal");

                if (Mathf.Abs(horizontalInput) > horizontalSensitivity && Time.time > lastMoveTime + navigationCooldown)
                {
                    lastMoveTime = Time.time;

                    // Zmień wybraną opcję w dropdown
                    int direction = horizontalInput > 0 ? 1 : -1;
                    int newValue = (dropdown.value + direction) % dropdown.options.Count;
                    if (newValue < 0) newValue = dropdown.options.Count - 1;

                    dropdown.value = newValue;

                    // Wywołaj zdarzenie onValueChanged
                    dropdown.onValueChanged.Invoke(dropdown.value);
                }
            }
        }
    }
    // Dodaj to do zmiennych klasy MainMenuManager:
    [Header("Ustawienia Sterowania")]
    [SerializeField] private bool enableControllerSupport = true;
    [SerializeField] private float controllerSensitivity = 0.5f;

    // Dodaj tę metodę do MainMenuManager:
    public void SetupControllerNavigation()
    {
        // Konfiguracja nawigacji UI dla kontrolera
        if (enableControllerSupport)
        {
            // Upewnij się, że wszystkie przyciski mają ustawioną nawigację
            Button[] allButtons = FindObjectsOfType<Button>();
            foreach (Button button in allButtons)
            {
                Navigation navigation = button.navigation;
                navigation.mode = Navigation.Mode.Automatic;
                button.navigation = navigation;
            }

            // Podobnie dla innych elementów UI
            Slider[] allSliders = FindObjectsOfType<Slider>();
            foreach (Slider slider in allSliders)
            {
                Navigation navigation = slider.navigation;
                navigation.mode = Navigation.Mode.Automatic;
                slider.navigation = navigation;
            }

            Toggle[] allToggles = FindObjectsOfType<Toggle>();
            foreach (Toggle toggle in allToggles)
            {
                Navigation navigation = toggle.navigation;
                navigation.mode = Navigation.Mode.Automatic;
                toggle.navigation = navigation;
            }

            Dropdown[] allDropdowns = FindObjectsOfType<Dropdown>();
            foreach (Dropdown dropdown in allDropdowns)
            {
                Navigation navigation = dropdown.navigation;
                navigation.mode = Navigation.Mode.Automatic;
                dropdown.navigation = navigation;
            }
        }
    }

}