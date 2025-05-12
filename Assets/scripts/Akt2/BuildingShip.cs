using System.Collections.Generic;
using UnityEngine;

public class BuildingShip : MonoBehaviour
{
    [SerializeField] private List<Renderer> shipParts;
    [SerializeField] private List<kloda> PutInShipParts;
    private Material WoodMaterial;
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
        if (currentPartIndex >= shipParts.Count)
        {

        }
    }
}
