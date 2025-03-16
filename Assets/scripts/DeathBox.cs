using UnityEngine;

public class DeathBox : MonoBehaviour
{
    [SerializeField] PlayerNumber playerNumber;
    public void DisableSelf()
    {
        gameObject.SetActive(false);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Player>(out var player))
        {
            if (player.playerNumber == playerNumber)
            {
                player.Die();
            }
        }
    }
}
