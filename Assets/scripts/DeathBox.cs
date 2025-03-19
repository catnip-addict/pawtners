using UnityEngine;

public class DeathBox : MonoBehaviour
{
    [SerializeField] PlayerNumber playerNumber;
    bool player1OnButton = false;
    bool player2OnButton = false;
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

    void OnTriggerExit(Collider other)
    {

        if (playerNumber == PlayerNumber.Both)
        {
            if (other.gameObject.TryGetComponent<Player>(out Player player))
            {
                if (player.playerNumber == PlayerNumber.First)
                {
                    player1OnButton = false;
                }
                if (player.playerNumber == PlayerNumber.Second)
                {
                    player2OnButton = false;
                }
            }
        }

    }
    private void OnTriggerStay(Collider other)
    {
        if (other.TryGetComponent<Player>(out var player))
        {
            if (player.playerNumber == playerNumber)
            {
                player.Die();
            }
            else if (playerNumber == PlayerNumber.Both)
            {

                if (player.playerNumber == PlayerNumber.First)
                {
                    player1OnButton = true;
                }
                if (player.playerNumber == PlayerNumber.Second)
                {
                    player2OnButton = true;
                }
                if (player1OnButton && player2OnButton)
                {
                    player.Die();
                }
            }
            else if (playerNumber == PlayerNumber.Whatever)
            {
                player.Die();
            }
        }
    }
}
