using UnityEngine;

public class OpenBrama : MonoBehaviour
{
    [SerializeField] private Animator animator1;
    [SerializeField] private Animator animator2;
    private Collider colliderBrama;

    private void Start()
    {
        colliderBrama = GetComponent<Collider>();
    }
    public void OpenBramaAnim()
    {
        animator1.SetBool("Opened", true);
        animator2.SetBool("Opened", true);
        colliderBrama.enabled = false;
    }
    public void CloseBramaAnim()
    {
        animator1.SetBool("Opened", false);
        animator2.SetBool("Opened", false);
        colliderBrama.enabled = true;
    }
}
