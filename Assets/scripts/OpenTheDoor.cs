using UnityEngine;

public class OpenTheDoor : MonoBehaviour
{
    [SerializeField] private Animator myDoor = null;
    [SerializeField] private bool openTrigger = false;
    [SerializeField] private CameraManager cameraManager;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Player>(out Player player))
        {
            myDoor.SetTrigger("otworz");
            cameraManager.PlayersExited();

            gameObject.SetActive(false);

        }
    }
}
