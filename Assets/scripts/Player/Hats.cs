using UnityEngine;

public class Hats : MonoBehaviour
{
    [SerializeReference] private GameObject[] hats;
    [SerializeReference] private PlayerNumber playerNumber;
    public int hatIndex = 0;
    void Start()
    {
        //PlayerPrefs.DeleteAll();
        if (TryGetComponent(out Player playerComponent))
        {
            playerNumber = playerComponent.playerNumber;
        }
        hatIndex = PlayerPrefs.GetInt("HatIndex" + playerNumber, 0);

        if (hats.Length > 0)
        {
            hats[hatIndex].SetActive(true);
        }
    }
    void Update()
    {

    }
    public void SetPlayerHat(int index, PlayerNumber playerNumber)
    {
        if (hats.Length == 0) return;
        for (int i = 0; i < hats.Length; i++)
        {
            hats[i].SetActive(false);
        }
        hats[index].SetActive(true);
        hatIndex = index;
        PlayerPrefs.SetInt("HatIndex" + playerNumber, hatIndex);
        PlayerPrefs.Save();
    }
    public int GetHatIndex()
    {
        return hatIndex;
    }
    public int GetHatLength()
    {
        return hats.Length;
    }
}
