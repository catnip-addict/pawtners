using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

[RequireComponent(typeof(TMP_Dropdown))]
public class DropdownSelect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler
{
    [Header("Animacje Dropdown")]
    [SerializeField] private float hoverScale = 1.1f;
    [SerializeField] private float animationSpeed = 10f;
    [SerializeField] private Color normalLabelColor = Color.black;
    [SerializeField] private Color hoverLabelColor = Color.black;
    [SerializeField] private Color expandedColor = new Color(1, 0.8f, 0);

    [Header("Dźwięki")]
    [SerializeField] private AudioClip hoverSound;
    [SerializeField] private AudioClip selectSound;

    private TMP_Dropdown dropdown;
    private TextMeshProUGUI dropdownLabel;
    private Image dropdownBackground;
    private AudioSource audioSource;
    private Vector3 originalScale;
    private Vector3 targetScale;
    private bool isExpanded = false;

    private void Awake()
    {
        dropdown = GetComponent<TMP_Dropdown>();

        dropdownLabel = transform.Find("Label")?.GetComponent<TextMeshProUGUI>();
        if (dropdownLabel == null)
        {
            Debug.LogWarning("Nie znaleziono etykiety dropdown. Upewnij się, że TMP_Dropdown ma standardową strukturę.");
        }

        dropdownBackground = GetComponent<Image>();

        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.volume = PlayerPrefs.GetFloat("SFXVolume", 1.0f);

        if (dropdownLabel != null)
        {
            originalScale = dropdownLabel.transform.localScale;
        }
        targetScale = originalScale;

        dropdown.onValueChanged.AddListener(OnDropdownValueChanged);
    }

    private void Update()
    {
        if (dropdownLabel != null)
        {
            dropdownLabel.transform.localScale = Vector3.Lerp(
                dropdownLabel.transform.localScale,
                targetScale,
                Time.deltaTime * animationSpeed
            );
        }

        audioSource.volume = PlayerPrefs.GetFloat("SFXVolume", 1.0f);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (dropdown.interactable)
        {
            targetScale = originalScale * hoverScale;

            if (dropdownLabel != null && !isExpanded)
            {
                dropdownLabel.color = hoverLabelColor;
            }

            if (!isExpanded)
            {
                // audioSource.PlayOneShot(hoverSound);
                SoundManager.Instance.PlayButtonSound();
            }
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!isExpanded)
        {
            targetScale = originalScale;

            if (dropdownLabel != null)
            {
                dropdownLabel.color = normalLabelColor;
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

    private void OnDropdownValueChanged(int value)
    {
        SoundManager.Instance.PlayClickSound();
    }

    private void OnDropdownShow()
    {
        isExpanded = true;

        if (dropdownLabel != null)
        {
            dropdownLabel.color = expandedColor;
        }
    }

    private void OnDropdownHide()
    {
        isExpanded = false;

        if (RectTransformUtility.RectangleContainsScreenPoint(
            GetComponent<RectTransform>(),
            Input.mousePosition,
            Camera.main))
        {
            if (dropdownLabel != null)
            {
                dropdownLabel.color = hoverLabelColor;
            }
        }
        else
        {
            targetScale = originalScale;
            if (dropdownLabel != null)
            {
                dropdownLabel.color = normalLabelColor;
            }
        }
    }
}
