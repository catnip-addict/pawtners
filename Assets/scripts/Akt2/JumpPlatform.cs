using UnityEngine;

public class JumpPlatform : MonoBehaviour
{
    [SerializeField] private float jumpForce = 20f;
    [SerializeField] private Animator animator;

    void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Player player))
        {
            animator.SetTrigger("Jump");
            player.JumpParticles();
            player.SetJumpVelocity(jumpForce);
            SoundManager.Instance.PlaySound(2);
            // Debug.Log("Jump");
        }
    }
}
