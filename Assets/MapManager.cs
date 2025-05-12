using System.Collections;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    [SerializeField] private GameObject[] Maps;
    [SerializeField] private Transform[] Entrence;
    [SerializeField] private Transform[] Exit;
    [SerializeField] private float teleportDelay = 1f;
    Player player1;
    Player player2;
    void Start()
    {
        for (int i = 0; i < Maps.Length; i++)
        {
            Maps[i].SetActive(false);
        }
        Maps[0].SetActive(true);
    }

    public void EnterMap(int mapIndex)
    {
        player1 = GameManager.Instance.player1;
        player2 = GameManager.Instance.player2;
        if (mapIndex < 0 || mapIndex >= Maps.Length)
        {
            Debug.LogError("Invalid map index: " + mapIndex);
            return;
        }
        StartCoroutine(ChangeMap(mapIndex, mapIndex - 1, Entrence));
    }
    public void ExitMap(int teleportIndex)
    {
        StartCoroutine(ChangeMap(0, teleportIndex, Exit));
    }
    private IEnumerator ChangeMap(int mapIndex, int teleportindex, Transform[] teleportPoint)
    {
        TransitionManager.Instance.FadeIn();
        yield return new WaitForSeconds(teleportDelay);
        player1.ZeroEverything();
        player2.ZeroEverything();
        player1.enabled = false;
        player2.enabled = false;
        player1.transform.position = teleportPoint[teleportindex].position;
        player2.transform.position = teleportPoint[teleportindex].position + new Vector3(0.5f, 0.5f, 0);
        yield return new WaitForSeconds(teleportDelay);
        for (int i = 0; i < Maps.Length; i++)
        {
            Maps[i].SetActive(false);
        }
        Maps[mapIndex].SetActive(true);
        // player1.TeleportToPosition(teleportPoint[teleportindex].position);

        // player2.TeleportToPosition(teleportPoint[teleportindex].position + new Vector3(0.5f, 0.5f, 0));
        yield return new WaitForSeconds(teleportDelay + 1f);
        player2.enabled = true;
        player1.enabled = true;
        player1.transform.position = teleportPoint[teleportindex].position;
        player2.transform.position = teleportPoint[teleportindex].position + new Vector3(0.5f, 0.5f, 0);
        player1.ZeroEverything();
        player2.ZeroEverything();
        TransitionManager.Instance.FadeOut();
    }

}
