using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TransitionManager : MonoBehaviour
{
    public static TransitionManager instance;

    [SerializeField] private CanvasGroup fadeCanvasGroup;
    [SerializeField] private GraphicRaycaster graphicRaycaster;
    [SerializeField] private float fadeDuration = 1.0f;

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

    public void FadeIn()
    {
        graphicRaycaster.enabled = true;
        StartCoroutine(Fade(0f, 1f));
    }

    public void FadeOut()
    {
        graphicRaycaster.enabled = false;
        StartCoroutine(Fade(1f, 0f));
    }

    private IEnumerator Fade(float startAlpha, float endAlpha)
    {
        if (startAlpha > endAlpha)
        {
            yield return new WaitForSeconds(1f);
        }
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            fadeCanvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / fadeDuration);
            yield return null;
        }

        fadeCanvasGroup.alpha = endAlpha;
    }

    public IEnumerator TransitionToScene(int sceneName)
    {
        graphicRaycaster.enabled = true;
        yield return StartCoroutine(Fade(0f, 1f));
        SceneManager.LoadScene(sceneName);
        graphicRaycaster.enabled = false;
        yield return StartCoroutine(Fade(1f, 0f));
    }
}