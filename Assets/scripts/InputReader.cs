using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using static Input_Actions;
public enum PlayerControllerType
{
    Keyboard,
    Gamepad
}
[CreateAssetMenu(fileName = "InputReader", menuName = "Platformer/InputReader")]
public class InputReader : MonoBehaviour, IKeyboardActions, IControllerActions
{
    public PlayerControllerType playerControllerType;
    public event UnityAction<Vector2> Move = delegate { };
    public event UnityAction<Vector2, bool> Look = delegate { };
    public event UnityAction EnableMouseControlCamera = delegate { };
    public event UnityAction DisableMouseControlCamera = delegate { };
    public event UnityAction<bool> Jump = delegate { };
    public event UnityAction<bool> Moew = delegate { };

    Input_Actions inputActions;

    public Vector3 Direction
    {
        get
        {
            if (playerControllerType == PlayerControllerType.Keyboard)
            {
                return inputActions.Keyboard.Walking.ReadValue<Vector2>();
            }
            else
            {
                return inputActions.Controller.Walking.ReadValue<Vector2>();
            }
        }
    }

    public void OnEnable()
    {
        if (inputActions == null)
        {
            inputActions = new Input_Actions();
        }
        if (playerControllerType == PlayerControllerType.Keyboard)
        {
            inputActions.Keyboard.SetCallbacks(this);
            inputActions.Controller.RemoveCallbacks(this);
        }
        else
        {
            inputActions.Controller.SetCallbacks(this);
            inputActions.Keyboard.RemoveCallbacks(this);
        }
    }

    public void EnablePlayerActions()
    {
        inputActions.Enable();
    }
    public void OnLook(InputAction.CallbackContext context)
    {
        Look.Invoke(context.ReadValue<Vector2>(), IsDeviceMouse(context));
    }

    bool IsDeviceMouse(InputAction.CallbackContext context) => context.control.device.name == "Mouse";

    public void OnFire(InputAction.CallbackContext context)
    {
        // noop
    }

    public void OnMouseControlCamera(InputAction.CallbackContext context)
    {
        switch (context.phase)
        {
            case InputActionPhase.Started:
                EnableMouseControlCamera.Invoke();
                break;
            case InputActionPhase.Canceled:
                DisableMouseControlCamera.Invoke();
                break;
        }
    }

    public void OnRun(InputAction.CallbackContext context)
    {
        // noop
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        switch (context.phase)
        {
            case InputActionPhase.Started:
                Jump.Invoke(true);

                break;
            case InputActionPhase.Canceled:
                Jump.Invoke(false);
                break;

        }

    }

    public void OnWalking(InputAction.CallbackContext context)
    {
        Move.Invoke(context.ReadValue<Vector2>());
    }

    public void OnPause(InputAction.CallbackContext context)
    {
        throw new System.NotImplementedException();
    }

    public void OnSound(InputAction.CallbackContext context)
    {
        Moew.Invoke(true);
    }
}