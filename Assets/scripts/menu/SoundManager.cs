using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }
    [Header("Źródła Audio")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;
    // public AudioClip[] musicClips;
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
        sfxSource = gameObject.AddComponent<AudioSource>();
        musicSource = gameObject.AddComponent<AudioSource>();
    }
    private void Start()
    {

        sfxSource.playOnAwake = false;
        sfxSource.volume = PlayerPrefs.GetFloat("SFXVolume", 1.0f);


        musicSource.playOnAwake = true;
        musicSource.volume = PlayerPrefs.GetFloat("MusicVolume", 1.0f);
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
            sfxSource.PlayOneShot(soundClips[clipIndex]);
        }
    }
    // public void PlayMusic(int clipIndex)
    // {
    //     if (clipIndex >= 0 && clipIndex < musicClips.Length)
    //     {
    //         musicSource.clip = musicClips[clipIndex];
    //         musicSource.loop = true;
    //         musicSource.Play();
    //     }
    // }
    public void StopSound()
    {
        sfxSource.Stop();
    }
    public void UpdateMusicVolume(float volume)
    {
        musicSource.volume = volume;
    }

    public void UpdateSFXVolume(float volume)
    {
        sfxSource.volume = volume;
    }
}
