using UnityEngine;

public class TeleportMe : MonoBehaviour
{
    public GameObject player;
    private void OnTriggerEnter(Collider other)
    {
        player = other.gameObject;
        player.transform.position = new Vector3(-35, 0.5f, 8);
    }
}
