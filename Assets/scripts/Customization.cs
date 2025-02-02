using UnityEngine;
using UnityEngine.UI;

public class Customization : MonoBehaviour
{
    [SerializeReference] private Player player;
    [SerializeReference] private Material[] materials;
    [SerializeReference] private GameObject[] hats;
    [SerializeReference] private RawImage colorImage;
    int colorIndex = 0;
    int hatIndex = 0;
    // public Player player2;

    public void NextHat()
    {
        hatIndex++;
        if (hatIndex >= hats.Length)
        {
            hatIndex = 0;
        }
        player.SetPlayerHat(hats[hatIndex]);
    }
    public void PreviousHat()
    {
        hatIndex--;
        if (hatIndex < 0)
        {
            hatIndex = hats.Length - 1;
        }
        player.SetPlayerHat(hats[hatIndex]);
    }
    public void NextColor()
    {
        colorIndex++;
        if (colorIndex >= materials.Length)
        {
            colorIndex = 0;
        }
        player.SetPlayerMat(materials[colorIndex]);
        colorImage.color = materials[colorIndex].color;
    }
    public void PreviousColor()
    {
        colorIndex--;
        if (colorIndex < 0)
        {
            colorIndex = materials.Length - 1;
        }
        player.SetPlayerMat(materials[colorIndex]);
        colorImage.color = materials[colorIndex].color;
    }
}
