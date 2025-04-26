using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ControllerSelectionVisualizer : MonoBehaviour
{
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color selectedColor = new Color(1, 0.8f, 0);
    [SerializeField] private float pulseSpeed = 1.5f;
    [SerializeField] private float pulseIntensity = 0.2f;

    private Selectable selectable;
    private Image image;
    private bool isSelected = false;

    private void Awake()
    {
        selectable = GetComponent<Selectable>();
        image = GetComponent<Image>();
    }

    private void Update()
    {
        // Sprawdź czy ten element jest aktualnie wybrany
        bool currentlySelected = (EventSystem.current.currentSelectedGameObject == gameObject);

        // Jeśli zmienił się stan zaznaczenia
        if (currentlySelected != isSelected)
        {
            isSelected = currentlySelected;

            // Zmień kolor
            if (image != null)
            {
                if (isSelected)
                {
                    // Dodaj pulsowanie
                    StartCoroutine(PulseAnimation());
                }
                else
                {
                    // Przywróć normalny kolor
                    image.color = normalColor;
                }
            }
        }
    }

    private System.Collections.IEnumerator PulseAnimation()
    {
        while (isSelected)
        {
            float pulse = (Mathf.Sin(Time.time * pulseSpeed) * pulseIntensity) + (1 - pulseIntensity);
            image.color = new Color(
                selectedColor.r * pulse,
                selectedColor.g * pulse,
                selectedColor.b * pulse,
                selectedColor.a
            );

            yield return null;
        }
    }
}