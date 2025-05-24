using System.Collections;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    [SerializeField] private GameObject[] Maps;
    [SerializeField] private Transform[] Entrence;
    [SerializeField] private Transform[] Exit;
    [SerializeField] private float teleportDelay = 1f;
    [SerializeField] private Vector3 player2Offset = new Vector3(2f, 0.5f, 0);

    private Player player1;
    private Player player2;
    private bool isTransitioning = false;

    void Start()
    {
        if (GameManager.Instance != null)
        {
            player1 = GameManager.Instance.player1;
            player2 = GameManager.Instance.player2;
        }

        SwitchToMap(0);
    }

    public void EnterMap(int mapIndex)
    {
        if (isTransitioning) return;
        if (player1 == null || player2 == null)
        {
            EnsurePlayerReferences();
        }

        if (!IsValidMapIndex(mapIndex))
        {
            return;
        }

        StartCoroutine(ChangeMap(mapIndex, mapIndex - 1, Entrence));
    }

    public void ExitMap(int teleportIndex)
    {
        if (isTransitioning) return;

        EnsurePlayerReferences();
        StartCoroutine(ChangeMap(0, teleportIndex, Exit));
    }

    private IEnumerator ChangeMap(int mapIndex, int teleportIndex, Transform[] teleportPoints)
    {
        if (!IsValidTeleportIndex(teleportIndex, teleportPoints))
        {
            yield break;
        }

        isTransitioning = true;

        TransitionManager.Instance.FadeIn();
        yield return new WaitForSeconds(teleportDelay);

        PreparePlayersForTeleport();

        SwitchToMap(mapIndex);

        Vector3 teleportPosition = teleportPoints[teleportIndex].position;
        PositionPlayers(teleportPosition);

        yield return new WaitForSeconds(teleportDelay + 1f);

        EnablePlayers();

        PositionPlayers(teleportPosition);

        ResetPlayerStates();

        TransitionManager.Instance.FadeOut();
        isTransitioning = false;
    }

    private void EnsurePlayerReferences()
    {
        if (player1 == null || player2 == null)
        {
            player1 = GameManager.Instance.player1;
            player2 = GameManager.Instance.player2;
        }
    }

    private bool IsValidMapIndex(int mapIndex)
    {
        return mapIndex >= 0 && mapIndex < Maps.Length;
    }

    private bool IsValidTeleportIndex(int teleportIndex, Transform[] teleportPoints)
    {
        return teleportIndex >= 0 && teleportIndex < teleportPoints.Length;
    }

    private void SwitchToMap(int mapIndex)
    {
        for (int i = 0; i < Maps.Length; i++)
        {
            Maps[i].SetActive(i == mapIndex);
        }
    }

    private void PreparePlayersForTeleport()
    {
        player1.ZeroEverything();
        player2.ZeroEverything();
        player1.enabled = false;
        player2.enabled = false;
        player1.DisableRb(true);
        player2.DisableRb(true);
    }

    private void PositionPlayers(Vector3 basePosition)
    {
        player1.transform.position = basePosition;
        player2.transform.position = basePosition + player2Offset;
    }

    private void EnablePlayers()
    {
        player1.enabled = true;
        player2.enabled = true;
    }

    private void ResetPlayerStates()
    {
        player1.ZeroEverything();
        player2.ZeroEverything();
        player1.DisableRb(false);
        player2.DisableRb(false);
    }
}