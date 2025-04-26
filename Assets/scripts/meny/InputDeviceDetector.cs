using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.EventSystems;

public class InputDeviceDetector : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject firstSelectedGamepadObject;
    [SerializeField] private InputSystemUIInputModule inputModule;

    [Header("Settings")]
    [SerializeField] private float gamepadDetectionThreshold = 0.1f;
    [SerializeField] private bool hideCursorOnGamepad = true;

    private bool _usingGamepad = false;
    private GameObject _lastSelectedObject;

    private void Awake()
    {
        // Automatycznie znajdź InputSystemUIInputModule jeśli nie został przypisany
        if (inputModule == null)
        {
            inputModule = GetComponent<InputSystemUIInputModule>();
            if (inputModule == null)
            {
                inputModule = FindFirstObjectByType<InputSystemUIInputModule>();
            }
        }

        // Domyślnie ustaw dla myszy/klawiatury
        SetUIForMouseKeyboard();
    }

    private void Update()
    {
        DetectInputDevice();

        // Zachowaj ostatnio wybrany element w trybie gamepada
        if (_usingGamepad && EventSystem.current.currentSelectedGameObject != null)
        {
            _lastSelectedObject = EventSystem.current.currentSelectedGameObject;
        }

        // Jeśli używamy gamepada, ale nic nie jest wybrane, przywróć ostatnio wybrany obiekt
        if (_usingGamepad && EventSystem.current.currentSelectedGameObject == null)
        {
            if (_lastSelectedObject != null && _lastSelectedObject.activeInHierarchy)
            {
                EventSystem.current.SetSelectedGameObject(_lastSelectedObject);
            }
            else if (firstSelectedGamepadObject != null)
            {
                EventSystem.current.SetSelectedGameObject(firstSelectedGamepadObject);
            }
        }
    }

    private void DetectInputDevice()
    {
        bool gamepadDetected = false;

        // Sprawdź czy używany jest gamepad
        if (Gamepad.current != null)
        {
            Vector2 leftStick = Gamepad.current.leftStick.ReadValue();
            Vector2 dpad = Gamepad.current.dpad.ReadValue();

            // Sprawdź czy którykolwiek z przycisków nawigacyjnych jest używany
            if (leftStick.magnitude > gamepadDetectionThreshold ||
                dpad.magnitude > gamepadDetectionThreshold ||
                Gamepad.current.aButton.wasPressedThisFrame ||
                Gamepad.current.bButton.wasPressedThisFrame ||
                Gamepad.current.xButton.wasPressedThisFrame ||
                Gamepad.current.yButton.wasPressedThisFrame)
            {
                gamepadDetected = true;
            }
        }

        // Sprawdź czy używana jest mysz
        bool mouseDetected = false;
        if (Mouse.current != null)
        {
            Vector2 mouseDelta = Mouse.current.delta.ReadValue();
            if (mouseDelta.magnitude > 0 || Mouse.current.leftButton.wasPressedThisFrame)
            {
                mouseDetected = true;
            }
        }

        // Sprawdź czy używana jest klawiatura do nawigacji
        bool keyboardNavDetected = false;
        if (Keyboard.current != null)
        {
            if (Keyboard.current.upArrowKey.wasPressedThisFrame ||
                Keyboard.current.downArrowKey.wasPressedThisFrame ||
                Keyboard.current.leftArrowKey.wasPressedThisFrame ||
                Keyboard.current.rightArrowKey.wasPressedThisFrame ||
                Keyboard.current.enterKey.wasPressedThisFrame ||
                Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                keyboardNavDetected = true;
            }
        }

        // Zmieniaj tryb tylko gdy wykryto określone urządzenie
        if (gamepadDetected && !_usingGamepad)
        {
            SetUIForGamepad();
        }
        else if ((mouseDetected || keyboardNavDetected) && _usingGamepad)
        {
            SetUIForMouseKeyboard();
        }
    }

    private void SetUIForMouseKeyboard()
    {
        _usingGamepad = false;
        Cursor.visible = true;
        // Możesz dodać więcej zmian specyficznych dla UI, jak zmiana wyglądu przycisków itp.
        Debug.Log("Przełączono na sterowanie myszą/klawiaturą");
    }

    private void SetUIForGamepad()
    {
        _usingGamepad = true;

        if (hideCursorOnGamepad)
        {
            Cursor.visible = false;
        }

        // Wybierz pierwszy element, jeśli żaden nie jest aktualnie wybrany
        if (EventSystem.current.currentSelectedGameObject == null)
        {
            if (_lastSelectedObject != null && _lastSelectedObject.activeInHierarchy)
            {
                EventSystem.current.SetSelectedGameObject(_lastSelectedObject);
            }
            else if (firstSelectedGamepadObject != null)
            {
                EventSystem.current.SetSelectedGameObject(firstSelectedGamepadObject);
            }
        }

        // Możesz dodać więcej zmian specyficznych dla UI, jak zmiana wyglądu przycisków itp.
        Debug.Log("Przełączono na sterowanie gamepadem");
    }
}