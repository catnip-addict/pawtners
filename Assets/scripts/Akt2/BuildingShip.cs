using System.Collections.Generic;
using UnityEngine;

public class BuildingShip : MonoBehaviour
{
    [SerializeField] private List<Renderer> shipParts;
    [SerializeField] private List<kloda> PutInShipParts;
    [SerializeField] private Animator animator;
    private Material WoodMaterial;
    bool isOnIsland = false;
    public int currentPartIndex = 0;

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
    public void SwimToIsland()
    {
        if (currentPartIndex >= shipParts.Count)
        {
            if (isOnIsland)
            {
                animator.SetBool("SwimTo", false);
                animator.SetBool("SwimBack", true);
            }
            else
            {
                animator.SetBool("SwimTo", true);
                animator.SetBool("SwimBack", false);
            }
        }
    }
}
