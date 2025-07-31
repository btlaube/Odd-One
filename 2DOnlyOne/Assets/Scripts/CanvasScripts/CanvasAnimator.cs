using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CanvasGroup), typeof(Canvas))]
public class CanvasAnimator : MonoBehaviour, ICanvasAnimator
{
    [SerializeField] private float fadeDuration = 0.5f;

    private CanvasGroup canvasGroup;
    private Canvas canvas;

    private bool isVisible = false;
    private bool isFading = false;

    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        canvas = GetComponent<Canvas>();

        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
        canvas.enabled = false;
    }

    public void Toggle()
    {
        if (isFading) return;

        if (isVisible)
            StartCoroutine(FadeOut());
        else
            StartCoroutine(FadeIn());
    }

    public void Show()
    {
        if (!isVisible && !isFading)
            StartCoroutine(FadeIn());
    }

    public void Hide()
    {
        if (isVisible && !isFading)
            StartCoroutine(FadeOut());
    }

    private IEnumerator FadeIn()
    {
        isFading = true;
        canvas.enabled = true;

        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsed / fadeDuration);
            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }

        canvasGroup.alpha = 1f;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;

        isVisible = true;
        isFading = false;
    }

    private IEnumerator FadeOut()
    {
        isFading = true;

        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }

        canvasGroup.alpha = 0f;
        canvas.enabled = false;

        isVisible = false;
        isFading = false;
    }

    public bool GetVisibility()
    {
        return isVisible;
    }
}
