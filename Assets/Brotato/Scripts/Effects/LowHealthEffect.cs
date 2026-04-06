using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class LowHealthEffect : MonoBehaviour
{
    [Header(" Settings ")]
    [SerializeField] private float lowHealthThreshold = 1f; // Kích hoạt khi máu <= 30%
    [SerializeField] private float maxDarkness = 0.6f;        // Độ mờ tối đa (0.0 đến 1.0)
    [SerializeField] private float fadeDuration = 0.5f;       // Thời gian chuyển đổi (giây)

    private CanvasGroup canvasGroup;
    private bool isDarkened = false;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        PlayerHealth.onHealthChanged += HandleHealthChanged;
    }

    private void OnDestroy()
    {
        PlayerHealth.onHealthChanged -= HandleHealthChanged;
    }

   private void HandleHealthChanged(float healthRatio)
    {
        // Thêm dòng log này để kiểm tra trên Console
        if (healthRatio <= lowHealthThreshold && !isDarkened)
        {
            isDarkened = true;
            LeanTween.cancel(gameObject);
            LeanTween.alphaCanvas(canvasGroup, maxDarkness, fadeDuration).setIgnoreTimeScale(true);
        }
        else if (healthRatio > lowHealthThreshold && isDarkened)
        {
            isDarkened = false;
            LeanTween.cancel(gameObject);
            LeanTween.alphaCanvas(canvasGroup, 0f, fadeDuration).setIgnoreTimeScale(true);
        }
    }
}