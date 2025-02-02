using UnityEngine;

public class Item : MonoBehaviour
{
    public Collider itemCollider;

    public void SetCollider(bool value)
    {
        itemCollider.enabled = value;
    }
}
