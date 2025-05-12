using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingShip : MonoBehaviour
{
    [SerializeField] private List<Renderer> shipParts;
    [SerializeField] private List<kloda> PutInShipParts;
    [SerializeField] private Animator animator;
    [SerializeField] private Transform teleportPoint;
    [SerializeField] private GameObject otherBoat;
<<<<<<< HEAD
    [SerializeField] private float teleportDelay = 1f;
    [SerializeField] private Material WoodMaterial;
=======
    private Material WoodMaterial;
>>>>>>> 7e5334aef6e0c561acbf4b766bcf25486ec6987c
    bool isOnIsland = false;
    Player player1;
    Player player2;
    public int currentPartIndex = 0;

    private void Start()
    {
        player1 = GameManager.Instance.player1;
        player2 = GameManager.Instance.player2;
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<kloda>(out kloda log))
        {
            if (PutInShipParts.Contains(log))
            {
                return;
            }
            shipParts[log.logIndex].material = WoodMaterial;
            PutInShipParts.Add(log);
            currentPartIndex++;
            // Destroy(other.gameObject);
            other.gameObject.SetActive(false);
        }
    }
<<<<<<< HEAD
    public void SwimToIsland()
    {
        if (currentPartIndex >= shipParts.Count)
        {
            StartCoroutine(SwimToIslandCoroutine());
        }
    }

    private IEnumerator SwimToIslandCoroutine()
    {
        player1 = GameManager.Instance.player1;
        player2 = GameManager.Instance.player2;

        TransitionManager.Instance.FadeIn();
        yield return new WaitForSeconds(teleportDelay);

        player1.TeleportToPosition(teleportPoint.position);
        player1.SetJumpVelocity(0);
        player2.SetJumpVelocity(0);
        player2.TeleportToPosition(teleportPoint.position + new Vector3(0.5f, 0.5f, 0));
        otherBoat.SetActive(true);
        yield return new WaitForSeconds(teleportDelay);
        TransitionManager.Instance.FadeOut();

        gameObject.SetActive(false);
    }

    public void SwimBackToIsland()
    {
        StartCoroutine(SwimBackToIslandCoroutine());
    }

    private IEnumerator SwimBackToIslandCoroutine()
    {
        player1 = GameManager.Instance.player1;
        player2 = GameManager.Instance.player2;

        TransitionManager.Instance.FadeIn();
        yield return new WaitForSeconds(teleportDelay);

        player1.TeleportToPosition(teleportPoint.position);
        player2.TeleportToPosition(teleportPoint.position + new Vector3(0.5f, 0.5f, 0));
        player1.SetJumpVelocity(0);
        player2.SetJumpVelocity(0);
        otherBoat.SetActive(true);


        yield return new WaitForSeconds(teleportDelay);
        TransitionManager.Instance.FadeOut();
        gameObject.SetActive(false);
    }
=======

>>>>>>> 7e5334aef6e0c561acbf4b766bcf25486ec6987c
}