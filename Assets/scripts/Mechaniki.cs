using UnityEngine;

public class Mechaniki : MonoBehaviour
{
    public Transform holdPosition;  // Pozycja, w której trzymamy przedmiot
    public float pickUpRange = 3f;  // Zasięg podnoszenia
    public LayerMask pickUpLayer;   // Warstwa podnoszonych obiektów

    private GameObject heldObject;
    private Rigidbody heldObjectRb;
    private Item heldObjectItem;
    private Player player;
    void Start()
    {
        player = GetComponent<Player>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (heldObject == null)
            {
                TryPickUp();
            }
            else
            {
                DropObject();
            }
        }
    }

    void TryPickUp()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, pickUpRange, pickUpLayer))
        {
            if (hit.collider.CompareTag("PickUp"))
            {
                heldObject = hit.collider.gameObject;
                heldObjectItem = heldObject.GetComponent<Item>();
                heldObjectRb = heldObject.GetComponent<Rigidbody>();

                if (heldObjectRb != null)
                {
                    heldObjectItem.SetCollider(false);
                    heldObject.transform.position = holdPosition.position;
                    heldObject.transform.rotation = holdPosition.rotation;
                    heldObject.transform.parent = holdPosition;
                    heldObjectRb.useGravity = false;
                    heldObjectRb.isKinematic = true;
                    // player.SetWeight(heldObjectItem.weight);
                }
            }
        }
    }

    void DropObject()
    {
        if (heldObjectRb != null)

        {
            heldObject.transform.parent = null;
            heldObjectItem.SetCollider(true);
            heldObjectRb.useGravity = true;
            heldObjectRb.isKinematic = false;
        }
        // player.SetWeight(0f);
        heldObject = null;
        heldObjectRb = null;
    }
}
