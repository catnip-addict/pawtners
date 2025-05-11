using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ButtonGrzyb : MonoBehaviour
{
    [SerializeField] PlayerNumber playerNumber;
    [SerializeField] private Animator animator;
    public UnityEvent onButtonPress;
    public UnityEvent offButtonPress;
    bool player1OnButton = false;
    bool player2OnButton = false;
    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.TryGetComponent<Player>(out Player player))
        {
            if (playerNumber == PlayerNumber.Both)
            {
                if (player.playerNumber == PlayerNumber.First)
                {
                    player1OnButton = false;
                }
                if (player.playerNumber == PlayerNumber.Second)
                {
                    player2OnButton = false;
                }
                if (!player1OnButton && !player2OnButton)  // Changed condition to check if both players are gone
                {
                    animator.SetBool("IsPressed", false);
                    StartCoroutine(InvokeOffButtonPressWithDelay(1f));
                    Debug.Log("Player exited the button");
                }
            }
            else
            {
                animator.SetBool("IsPressed", false);
                StartCoroutine(InvokeOffButtonPressWithDelay(1f));
                Debug.Log("Player exited the button");
            }
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.TryGetComponent<Player>(out var player))
        {
            if (player.playerNumber == playerNumber)
            {
                if (onButtonPress != null)
                {
                    onButtonPress.Invoke();
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
                if (player1OnButton && player2OnButton)
                {
                    if (onButtonPress != null)
                    {
                        onButtonPress.Invoke();
                    }
                }
            }
            else if (playerNumber == PlayerNumber.Whatever)
            {
                if (onButtonPress != null)
                {
                    onButtonPress.Invoke();
                }
            }
            animator.SetBool("IsPressed", true);
        }
    }
    private IEnumerator InvokeOffButtonPressWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        offButtonPress.Invoke();
    }
}
