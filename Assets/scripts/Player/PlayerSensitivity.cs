using Unity.Cinemachine;
using UnityEngine;

public class PlayerSensitivity : MonoBehaviour
{
    public CinemachineInputAxisController cinemamachineMouse;
    public CinemachineInputAxisController cinemachineController;
    float mouseSens;
    float jouStickSens;

    public void SetMouseSensitivity()
    {
        mouseSens = PlayerPrefs.GetFloat("MouseSens", 1.0f);
        foreach (var controller in cinemamachineMouse.Controllers)
        {
            switch (controller.Name)
            {
                case "Look Orbit X":
                    controller.Input.LegacyGain = mouseSens;
                    controller.Input.Gain = 2f * mouseSens;
                    break;
                case "Look Orbit Y":
                    controller.Input.LegacyGain = -mouseSens;
                    controller.Input.Gain = -2f * mouseSens;
                    break;
            }
        }
    }
    public void SetJoyStickSensitivity()
    {
        jouStickSens = PlayerPrefs.GetFloat("JoystickSens", 1.0f);
        foreach (var controller in cinemachineController.Controllers)
        {
            switch (controller.Name)
            {
                case "Look Orbit X":
                    controller.Input.LegacyGain = jouStickSens;
                    controller.Input.Gain = 2f * jouStickSens;
                    break;
                case "Look Orbit Y":
                    controller.Input.LegacyGain = -jouStickSens;
                    controller.Input.Gain = -2f * jouStickSens;
                    break;
            }
        }
    }
}
