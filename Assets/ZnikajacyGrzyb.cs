using UnityEngine;

public class ZnikajacyGrzyb : MonoBehaviour
{
    [SerializeField] private float disappearTime = 1f;
    [SerializeField] private Animator animator;
    [SerializeField] private Collider grzybCollider;

    private void Start()
    {
        gameObject.SetActive(true);
    }

    public void Disappear()
    {
        animator.SetTrigger("IsPressed");

        Invoke(nameof(DisableMushroom), disappearTime);
    }

    private void DisableMushroom()
    {
        animator.SetTrigger("Disappear");
        grzybCollider.enabled = false;
        Invoke(nameof(ResetMushroom), disappearTime * 4);
    }
    public void ResetMushroom()
    {
        animator.SetTrigger("Reset");
        grzybCollider.enabled = true;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Player player))
        {
            Disappear();
            // Debug.Log("Player collided with mushroom");
        }
    }
}
