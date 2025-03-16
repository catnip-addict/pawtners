using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public enum PlayerScreen
{
    left_right,
    right_left
}
public class CameraManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Camera player1Cam;
    [SerializeField] Camera player2Cam;

    [Header("Settings")]
    [SerializeField] PlayerScreen playerScreenPos;

    // InputReader input;

    void OnValidate()
    {
        if (playerScreenPos == PlayerScreen.left_right)
        {
            player1Cam.rect = new Rect(0, 0, 0.5f, 1);
            player2Cam.rect = new Rect(0.5f, 0, 0.5f, 1);
        }
        else
        {
            player1Cam.rect = new Rect(0.5f, 0, 0.5f, 1);
            player2Cam.rect = new Rect(0, 0, 0.5f, 1);
        }
    }
}