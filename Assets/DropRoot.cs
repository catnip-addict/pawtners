using UnityEngine;
using System.Collections;

public class DropRoot : MonoBehaviour
{
    [SerializeField] Animator animator;
    bool isDown = false;
    float delay = 0.5f;

    public void Drop()
    {
        isDown = true;
        StartCoroutine(DelayedDrop());
    }

    public void Up()
    {
        isDown = false;
        StartCoroutine(DelayedUp());
    }

    private IEnumerator DelayedDrop()
    {
        yield return new WaitForSeconds(delay);
        animator.SetBool("Down", true);
    }

    private IEnumerator DelayedUp()
    {
        yield return new WaitForSeconds(delay);
        if (isDown)
        {
            animator.SetBool("Down", true);
        }
        else
        {
            animator.SetBool("Down", false);
        }
    }
}