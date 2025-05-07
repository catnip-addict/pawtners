using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }
    public AudioSource audioSource;
    public AudioClip[] soundClips;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(this.gameObject);

        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.volume = PlayerPrefs.GetFloat("SFXVolume", 1.0f);
    }
    public void PlayButtonSound()
    {
        PlaySound(0);
    }
    public void PlayClickSound()
    {
        PlaySound(1);
    }
    public void PlaySound(int clipIndex)
    {
        if (clipIndex >= 0 && clipIndex < soundClips.Length)
        {
            audioSource.PlayOneShot(soundClips[clipIndex]);
            Debug.Log("Playing sound clip: " + soundClips[clipIndex].name);
        }
        else
        {
            Debug.LogWarning("Sound clip index out of range: " + clipIndex);
        }
    }
    public void StopSound()
    {
        audioSource.Stop();
    }
}
