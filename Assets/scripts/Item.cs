using UnityEngine;

public class Item : MonoBehaviour
{
    public Collider itemCollider;
    [SerializeField]
    Collider triggerCollider;
    public float weight = 0f;
    public bool needAnimation = true;

    public void SetCollider(bool value)
    {
        itemCollider.enabled = value;
        if (triggerCollider != null)
        {
            triggerCollider.enabled = value;
        }
    }

    public void MyszkaDodaj()
    {
        if (TryGetComponent<Myszka>(out Myszka myszka))
        {
            GameManager.Instance.mouseCont += 1;
        }
    }
}
