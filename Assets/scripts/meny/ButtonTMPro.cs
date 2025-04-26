using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

[RequireComponent(typeof(Button))]
public class ButtonTMPro : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler
{
    [Header("Animacje Tekstu")]
    [SerializeField] private float hoverScale = 1.1f;
    [SerializeField] private float animationSpeed = 10f;
    [SerializeField] private Color normalTextColor = Color.white;
    [SerializeField] private Color hoverTextColor = Color.yellow;
    [SerializeField] private Color pressedTextColor = new Color(1, 0.6f, 0);

    [Header("Dźwięki")]
    [SerializeField] private AudioClip hoverSound;
    [SerializeField] private AudioClip clickSound;

    private Button button;
    private TextMeshProUGUI buttonText;
    private AudioSource audioSource;
    private Vector3 originalScale;
    private Vector3 targetScale;

    private void Awake()
    {
        button = GetComponent<Button>();
        buttonText = GetComponentInChildren<TextMeshProUGUI>();

        // Stwórz audio source dla tego przycisku
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.volume = PlayerPrefs.GetFloat("SFXVolume", 1.0f);

        // Zapisz oryginalną skalę
        if (buttonText != null)
        {
            originalScale = buttonText.transform.localScale;
        }
        targetScale = originalScale;

        // Dodaj listener kliknięcia
        button.onClick.AddListener(OnButtonClick);
    }

    private void Update()
    {
        // Płynna animacja skalowania
        if (buttonText != null)
        {
            buttonText.transform.localScale = Vector3.Lerp(
                buttonText.transform.localScale,
                targetScale,
                Time.deltaTime * animationSpeed
            );
        }

        // Aktualizacja głośności
        audioSource.volume = PlayerPrefs.GetFloat("SFXVolume", 1.0f);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (button.interactable)
        {
            // Powiększamy tekst
            targetScale = originalScale * hoverScale;

            // Zmieniamy kolor tekstu
            if (buttonText != null)
            {
                buttonText.color = hoverTextColor;
            }

            // Odtwarzamy dźwięk
            if (hoverSound != null)
            {
                audioSource.PlayOneShot(hoverSound);
            }
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // Przywracamy normalny stan
        targetScale = originalScale;

        if (buttonText != null)
        {
            buttonText.color = normalTextColor;
        }
    }

    public void OnSelect(BaseEventData eventData)
    {
        // Podobnie jak najechanie myszą
        OnPointerEnter(eventData as PointerEventData);
    }

    public void OnDeselect(BaseEventData eventData)
    {
        // Podobnie jak opuszczenie kursorem
        OnPointerExit(eventData as PointerEventData);
    }

    private void OnButtonClick()
    {
        // Efekt wciśnięcia
        if (buttonText != null)
        {
            buttonText.color = pressedTextColor;
            // Można dodać krótką animację
            StartCoroutine(PressAnimation());
        }

        // Odtwarzamy dźwięk kliknięcia
        if (clickSound != null)
        {
            audioSource.PlayOneShot(clickSound);
        }
    }

    private System.Collections.IEnumerator PressAnimation()
    {
        Vector3 pressedScale = originalScale * 0.9f;
        buttonText.transform.localScale = pressedScale;

        yield return new WaitForSeconds(0.1f);

        targetScale = originalScale * hoverScale;

        // Przywracamy kolor po krótkim czasie
        yield return new WaitForSeconds(0.1f);
        buttonText.color = hoverTextColor;
    }
}