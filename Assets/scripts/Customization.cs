using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Customization : MonoBehaviour
{
    // [SerializeReference] private Player player1;
    // [SerializeReference] private Player player2;
    [SerializeReference] private Hats hatPlayer1;
    [SerializeReference] private TMP_Text hatTextPlayer2;
    [SerializeReference] private TMP_Text hatTextPlayer1;
    [SerializeReference] private Hats hatPlayer2;
    [SerializeReference] private Material[] materials;
    // [SerializeReference] private RawImage colorImage;
    int colorIndex = 0;
    int hatIndex = 0;
    int hatsLength = 0;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        hatsLength = hatPlayer1.GetHatLength();
        RefreshText();
    }

    public void NextHat(int playerNumber)
    {
        if (playerNumber == 1)
        {
            hatIndex = hatPlayer1.hatIndex;
            hatIndex++;
            if (hatIndex >= hatsLength)
            {
                hatIndex = 0;
            }
            hatPlayer1.SetPlayerHat(hatIndex, PlayerNumber.First);
        }
        else if (playerNumber == 2)
        {
            hatIndex = hatPlayer2.hatIndex;
            hatIndex++;
            if (hatIndex >= hatsLength)
            {
                hatIndex = 0;
            }
            hatPlayer2.SetPlayerHat(hatIndex, PlayerNumber.Second);
        }
        RefreshText();
    }
    public void PreviousHat(int playerNumber)
    {
        if (playerNumber == 1)
        {
            hatIndex--;
            if (hatIndex < 0)
            {
                hatIndex = hatsLength - 1;
            }
            hatPlayer1.SetPlayerHat(hatIndex, PlayerNumber.First);
        }
        else if (playerNumber == 2)
        {
            hatIndex--;
            if (hatIndex < 0)
            {
                hatIndex = hatsLength - 1;
            }
            hatPlayer2.SetPlayerHat(hatIndex, PlayerNumber.Second);
        }
        RefreshText();
    }
    public void RefreshText()
    {
        hatTextPlayer1.text = "Hat: " + (hatPlayer1.hatIndex + 1).ToString() + "/" + hatsLength.ToString();
        hatTextPlayer2.text = "Hat: " + (hatPlayer2.hatIndex + 1).ToString() + "/" + hatsLength.ToString();
    }
    // public void NextColor(PlayerNumber playerNumber)
    // {
    //     if (playerNumber == PlayerNumber.First)
    //     {
    //         colorIndex++;
    //         if (colorIndex >= materials.Length)
    //         {
    //             colorIndex = 0;
    //         }
    //         hatPlayer1.SetPlayerMat(materials[colorIndex]);
    //     }
    //     else if (playerNumber == PlayerNumber.Second)
    //     {
    //         colorIndex++;
    //         if (colorIndex >= materials.Length)
    //         {
    //             colorIndex = 0;
    //         }
    //         hatPlayer2.SetPlayerMat(materials[colorIndex]);
    //     }
    // }
    // public void PreviousColor(PlayerNumber playerNumber)
    // {
    //     if (playerNumber == PlayerNumber.First)
    //     {
    //         colorIndex--;
    //         if (colorIndex < 0)
    //         {
    //             colorIndex = materials.Length - 1;
    //         }
    //         hatPlayer1.SetPlayerMat(materials[colorIndex]);
    //     }
    //     else if (playerNumber == PlayerNumber.Second)
    //     {
    //         colorIndex--;
    //         if (colorIndex < 0)
    //         {
    //             colorIndex = materials.Length - 1;
    //         }
    //         hatPlayer2.SetPlayerMat(materials[colorIndex]);
    //     }
    // }
}
