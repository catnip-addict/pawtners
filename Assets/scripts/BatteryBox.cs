using UnityEngine.Events;
using UnityEngine;

public class BatteryBox : MonoBehaviour
{
    [SerializeField] Transform giveBack;
    [SerializeField] Transform holdPosition;
    [SerializeField] BatteryType neededBatteryType;
    public UnityEvent onBatteryPutIn;
    public UnityEvent onBatteryTakeOut;
    Rigidbody heldObjectRb;
    GameObject heldObject;
    Item heldObjectItem;
    // public void DropObject()
    // {
    //     if (heldObjectRb != null)
    //     {
    //         heldObject.transform.parent = null;
    //         heldObjectItem.SetCollider(true);
    //         heldObjectRb.useGravity = true;
    //         heldObjectRb.isKinematic = false;
    //         heldObject.transform.SetPositionAndRotation(giveBack.position, giveBack.rotation);
    //     }
    //     heldObject = null;
    //     heldObjectRb = null;
    // }
    public GameObject GiveBattery()
    {
        onBatteryTakeOut?.Invoke();
        return heldObject;
    }
    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("PickUp") && other.GetComponent<Item>().needAnimation)
        {
            heldObject = other.gameObject;
            heldObjectRb = heldObject.GetComponent<Rigidbody>();
            heldObjectItem = heldObject.GetComponent<Item>();
            heldObjectRb.useGravity = false;
            heldObjectRb.isKinematic = true;
            heldObject.transform.SetPositionAndRotation(holdPosition.position, holdPosition.rotation);
            heldObject.transform.parent = holdPosition;
            Battery battery = heldObject.GetComponent<Battery>();
            if (battery.batteryType == neededBatteryType)
            {
                onBatteryPutIn?.Invoke();
            }
        }
    }
}
