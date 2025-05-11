using UnityEngine;

public class MusicManager : MonoBehaviour
{
    private AudioSource audioSource;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        if (audioSource.isPlaying) return;

        audioSource.loop = true;
        audioSource.Play();
    }
}
