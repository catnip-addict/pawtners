// AudioManager.cs
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [Header("Źródła Audio")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("Klipy dźwiękowe")]
    [SerializeField] private AudioClip mainMenuMusic;
    [SerializeField] private AudioClip buttonClickSFX;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        musicSource.volume = PlayerPrefs.GetFloat("MusicVolume", 1.0f);
        sfxSource.volume = PlayerPrefs.GetFloat("SFXVolume", 1.0f);

        PlayMusic(mainMenuMusic);
    }

    public void PlayMusic(AudioClip clip)
    {
        musicSource.clip = clip;
        musicSource.Play();
    }

    public void PlaySFX(AudioClip clip)
    {
        sfxSource.PlayOneShot(clip);
    }

    public void PlayButtonClick()
    {
        PlaySFX(buttonClickSFX);
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