using UnityEngine;

public class ZnikajacyGrzybChild : MonoBehaviour
{
    public bool isPlayerOnButton = false;
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.TryGetComponent(out Player player))
        {
            isPlayerOnButton = true;
        }
    }
    private void OnCollisionExit(Collision other)
    {
        if (other.gameObject.TryGetComponent(out Player player))
        {
            isPlayerOnButton = false;
        }
    }
}
