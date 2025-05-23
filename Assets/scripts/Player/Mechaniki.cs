using UnityEngine;

public class Mechaniki : MonoBehaviour
{
    public Transform holdPositionAnim;  // Pozycja, w której trzymamy przedmiot z animacją
    public Transform holdPosition;  // Pozycja, w której trzymamy przedmiot i bez
    public float pickUpRange = 3f;  // Zasięg podnoszenia
    public LayerMask pickUpLayer;   // Warstwa podnoszonych obiektów

    private GameObject heldObject;
    private Rigidbody heldObjectRb;
    private Item heldObjectItem;
    private Player player;
    public bool haveObject;

    [SerializeField] private Outline outline;
    void Start()
    {
        player = GetComponent<Player>();
    }

    // void Update()
    // {
    //     if (Input.GetKeyDown(KeyCode.E))
    //     {
    //         if (heldObject == null)
    //         {
    //             TryPickUp();
    //         }
    //         else
    //         {
    //             DropObject();
    //         }
    //     }
    // }
    public void PickUpObject()
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
    void TryPickUp()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, pickUpRange, pickUpLayer))
        {
            if (hit.collider.CompareTag("PickUp"))
            {
                haveObject = true;
                outline = hit.collider.GetComponent<Outline>();
                outline.enabled = false;
                heldObject = hit.collider.gameObject;
                heldObjectItem = heldObject.GetComponent<Item>();
                GameManager.Instance.AddToMouse();
                if (!heldObjectItem.dragging)
                {
                    player.isRestricted = false;
                }
                else
                {
                    player.isRestricted = true;
                }
                heldObjectRb = heldObject.GetComponent<Rigidbody>();

                if (heldObjectRb != null)
                {
                    heldObjectItem.SetCollider(false);
                    if (heldObjectItem.needAnimation)
                    {
                        heldObject.transform.position = holdPositionAnim.position;
                        heldObject.transform.rotation = holdPositionAnim.rotation;
                        heldObject.transform.parent = holdPositionAnim;
                    }
                    else
                    {
                        heldObject.transform.position = holdPosition.position;
                        heldObject.transform.rotation = holdPosition.rotation;
                        heldObject.transform.parent = holdPosition;
                    }
                    heldObjectRb.useGravity = false;
                    heldObjectRb.isKinematic = true;
                    player.SetWeight(heldObjectItem.weight);
                }
            }
            else if (hit.collider.CompareTag("BatteryBox"))
            {
                GiveObject(hit.collider.GetComponent<BatteryBox>().GiveBattery());
                hit.collider.GetComponent<BatteryBox>().DeleteBattery();
            }
        }
    }

    public void GiveObject(GameObject item)
    {
        heldObject = item;
        heldObjectRb = item.GetComponent<Rigidbody>();
        heldObjectItem = item.GetComponent<Item>();
        heldObjectItem.SetCollider(false);
        heldObjectRb.useGravity = false;
        heldObjectRb.isKinematic = true;
        item.transform.SetPositionAndRotation(holdPosition.position, holdPosition.rotation);
        item.transform.parent = holdPosition;
    }
    void DropObject()
    {
        haveObject = false;
        if (heldObjectRb != null)

        {
            heldObject.transform.parent = null;
            heldObjectItem.SetCollider(true);
            heldObjectRb.useGravity = true;
            heldObjectRb.isKinematic = false;
        }
        outline.enabled = true;
        player.SetWeight(0f);
        heldObject = null;
        heldObjectRb = null;
        player.isRestricted = false;
        if (outline != null)
        {
            outline.enabled = true;
        }
    }
}
