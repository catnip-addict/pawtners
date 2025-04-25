using UnityEngine;

public class OpenTheDoor : MonoBehaviour
{
    [SerializeField] private Animator myDoor = null;
    [SerializeField] private bool openTrigger = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Player>(out Player player))
        {
            myDoor.SetTrigger("otworz");
            gameObject.SetActive(false);
        }
    }
}
