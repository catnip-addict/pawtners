using Unity.Cinemachine;
using UnityEngine;

public class DEBUGMOVMENT : MonoBehaviour
{
    public InputReader player1;
    public InputReader player2;

    //First player
    public CinemachineInputAxisController cinemachineInputAxisController1;
    public CinemachineInputAxisController cinemachineInputAxisController2;

    //Second player
    public CinemachineInputAxisController cinemachineInputAxisController3;
    public CinemachineInputAxisController cinemachineInputAxisController4;

    public void SwitchPlayers()
    {
        if (player1.playerControllerType == PlayerControllerType.Keyboard)
        {
            player1.playerControllerType = PlayerControllerType.Gamepad;
            player2.playerControllerType = PlayerControllerType.Keyboard;

            cinemachineInputAxisController1.enabled = false;
            cinemachineInputAxisController2.enabled = true;

            cinemachineInputAxisController3.enabled = false;
            cinemachineInputAxisController4.enabled = true;
        }
        else
        {
            player1.playerControllerType = PlayerControllerType.Keyboard;
            player2.playerControllerType = PlayerControllerType.Gamepad;

            cinemachineInputAxisController1.enabled = true;
            cinemachineInputAxisController2.enabled = false;

            cinemachineInputAxisController3.enabled = true;
            cinemachineInputAxisController4.enabled = false;
        }
        player1.OnEnable();
        player2.OnEnable();
        player1.EnablePlayerActions();
        player2.EnablePlayerActions();
    }
}
