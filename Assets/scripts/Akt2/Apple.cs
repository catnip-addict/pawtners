using UnityEngine;

public class Apple : MonoBehaviour
{
    [SerializeField] private Transform spawnPoint;
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("BatteryBox"))
        {
            transform.position = spawnPoint.position;
        }
    }
}
