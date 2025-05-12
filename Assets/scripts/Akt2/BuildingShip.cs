using System.Collections.Generic;
using UnityEngine;

public class BuildingShip : MonoBehaviour
{
    [SerializeField] private List<Renderer> shipParts;
    [SerializeField] private List<kloda> PutInShipParts;
    [SerializeField] private Animator animator;
    [SerializeField] private Transform teleportPoint;
    [SerializeField] private GameObject otherBoat;
    private Material WoodMaterial;
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
        if (other.CompareTag("Island"))
        {
            isOnIsland = true;
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Island"))
        {
            isOnIsland = false;
        }
    }

}