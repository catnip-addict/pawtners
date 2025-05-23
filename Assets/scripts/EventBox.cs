using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class EventBox : MonoBehaviour
{
    [SerializeField] PlayerNumber playerNumber;
    [SerializeField] bool disappearAfter = true;
    public UnityEvent onButtonPress;
    bool player1OnButton = false;
    bool player2OnButton = false;
    private bool eventTriggered = false;

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
                if (!player1OnButton && !player2OnButton)
                {
                    eventTriggered = false;
                }
            }
        }
        else if (playerNumber != PlayerNumber.Both)
        {
            eventTriggered = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Player>(out var player))
        {
            if (player.playerNumber == playerNumber && !eventTriggered)
            {
                if (onButtonPress != null)
                {
                    eventTriggered = true;
                    onButtonPress.Invoke();
                    gameObject.SetActive(!disappearAfter);
                }
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
                if (player1OnButton && player2OnButton && !eventTriggered)
                {
                    if (onButtonPress != null)
                    {
                        eventTriggered = true;
                        onButtonPress.Invoke();
                        gameObject.SetActive(!disappearAfter);
                    }
                }
            }
            else if (playerNumber == PlayerNumber.Whatever && !eventTriggered)
            {
                if (onButtonPress != null)
                {
                    eventTriggered = true;
                    onButtonPress.Invoke();
                    gameObject.SetActive(!disappearAfter);
                }
            }
        }
    }
}

