using UnityEngine;

public class Item : MonoBehaviour
{
    public Collider itemCollider;
    public float weight = 0f;

    public void SetCollider(bool value)
    {
        itemCollider.enabled = value;
    }
}
