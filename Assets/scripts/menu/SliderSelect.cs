using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Slider))]
public class SliderSelect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler
{
    [Header("Animacje Uchwytu")]
    [SerializeField] private float hoverScale = 1.3f;
    [SerializeField] private float animationSpeed = 10f;
    [SerializeField] private Color normalHandleColor = Color.white;
    [SerializeField] private Color hoverHandleColor = Color.white;

    [Header("Dźwięki")]
    [SerializeField] private AudioClip hoverSound;
    [SerializeField] private AudioClip slideSound;

    private Slider slider;
    private RectTransform sliderHandle;
    private Image handleImage;
    private AudioSource audioSource;
    private Vector3 originalScale;
    private Vector3 targetScale;
    private bool isDragging = false;

    private void Awake()
    {
        slider = GetComponent<Slider>();

        Transform handleTransform = transform.Find("Handle Slide Area/Handle");
        if (handleTransform != null)
        {
            sliderHandle = handleTransform.GetComponent<RectTransform>();
            handleImage = handleTransform.GetComponent<Image>();
        }
        else
        {
            Debug.LogWarning("Nie znaleziono uchwytu slidera. Upewnij się, że ma on standardową strukturę Unity UI Slider.");
        }

        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.volume = PlayerPrefs.GetFloat("SFXVolume", 1.0f);

        if (sliderHandle != null)
        {
            originalScale = sliderHandle.localScale;
        }
        targetScale = originalScale;

        slider.onValueChanged.AddListener(OnSliderValueChanged);
    }

    private void Update()
    {
        if (sliderHandle != null)
        {
            sliderHandle.localScale = Vector3.Lerp(
                sliderHandle.localScale,
                targetScale,
                Time.deltaTime * animationSpeed
            );
        }

        audioSource.volume = PlayerPrefs.GetFloat("SFXVolume", 1.0f);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (slider.interactable)
        {
            targetScale = originalScale * hoverScale;

            if (handleImage != null)
            {
                handleImage.color = hoverHandleColor;
            }

            if (hoverSound != null && !isDragging)
            {
                audioSource.PlayOneShot(hoverSound);
            }
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!isDragging)
        {
            targetScale = originalScale;

            if (handleImage != null)
            {
                handleImage.color = normalHandleColor;
            }
        }
    }

    public void OnSelect(BaseEventData eventData)
    {
        OnPointerEnter(eventData as PointerEventData);
    }

    public void OnDeselect(BaseEventData eventData)
    {
        OnPointerExit(eventData as PointerEventData);
    }

    private void OnSliderValueChanged(float value)
    {
        // Odtwarzamy dźwięk przeciągania
        if (slideSound != null && isDragging)
        {
            if (!audioSource.isPlaying || audioSource.clip != slideSound)
            {
                audioSource.clip = slideSound;
                audioSource.Play();
            }
        }
    }

    public void OnBeginDrag()
    {
        isDragging = true;
    }

    public void OnEndDrag()
    {
        isDragging = false;

        if (!RectTransformUtility.RectangleContainsScreenPoint(
            GetComponent<RectTransform>(),
            Input.mousePosition,
            Camera.main))
        {
            targetScale = originalScale;
            if (handleImage != null)
            {
                handleImage.color = normalHandleColor;
            }
        }
    }
}
