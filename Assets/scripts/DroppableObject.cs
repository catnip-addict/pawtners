using UnityEngine;

public class DroppableObject : MonoBehaviour
{
    [SerializeField] private int collisionSoundIndex = 0;
    [SerializeField] private float soundCooldown = 0.2f;

    private SoundManager soundManager;
    private float lastSoundTime = -Mathf.Infinity;

    private void Start()
    {
        GameObject gm = GameObject.Find("GameManager");
        if (gm != null)
        {
            soundManager = gm.GetComponent<SoundManager>();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (soundManager != null && Time.time - lastSoundTime >= soundCooldown)
        {
            soundManager.PlaySound(collisionSoundIndex);
            lastSoundTime = Time.time;
        }
    }
}
