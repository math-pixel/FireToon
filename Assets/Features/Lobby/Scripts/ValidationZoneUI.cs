using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class ValidationZoneUI : MonoBehaviour
{
    [Header("UI References")] public TMP_Text statusText;
    public TMP_Text playerCountText;
    public TMP_Text countdownText;
    public Slider progressSlider;
    public Image progressFill;

    [Header("Colors")] public Color waitingColor = Color.red;
    public Color validatingColor = Color.yellow;
    public Color readyColor = Color.green;

    private ValidationZone validationZone;
    private Coroutine countdownUICoroutine;

    void Start()
    {
        validationZone = FindObjectOfType<ValidationZone>();

        if (validationZone != null)
        {
            // Subscribe to events
            validationZone.onPlayerCountChanged.AddListener(UpdatePlayerCount);
            validationZone.onValidationStart.AddListener(OnValidationStart);
            validationZone.onValidationComplete.AddListener(OnValidationComplete);
            validationZone.onValidationCancelled.AddListener(OnValidationCancelled);
        }

        InitializeUI();
    }

    private void InitializeUI()
    {
        UpdatePlayerCount(0, validationZone?.RequiredPlayers ?? 1);
        UpdateStatus("Waiting for players...", waitingColor);

        if (progressSlider != null)
        {
            progressSlider.value = 0f;
            progressSlider.gameObject.SetActive(false);
        }

        if (countdownText != null)
        {
            countdownText.gameObject.SetActive(false);
        }
    }

    private void UpdatePlayerCount(int current, int required)
    {
        if (playerCountText != null)
        {
            playerCountText.text = $"Players: {current}/{required}";

            // Animate text when count changes
            playerCountText.transform.DOPunchScale(Vector3.one * 0.1f, 0.3f);
        }

        // Update status based on player count
        if (current >= required)
        {
            UpdateStatus("All players ready!", readyColor);
        }
        else
        {
            UpdateStatus("Waiting for players...", waitingColor);
        }
    }

    private void UpdateStatus(string message, Color color)
    {
        if (statusText != null)
        {
            statusText.text = message;
            statusText.color = color;
        }
    }

    private void OnValidationStart()
    {
        UpdateStatus("Validation starting...", validatingColor);

        if (progressSlider != null)
        {
            progressSlider.gameObject.SetActive(true);
            progressSlider.value = 0f;
        }

        if (countdownText != null)
        {
            countdownText.gameObject.SetActive(true);
        }

        // Start countdown UI
        float duration = validationZone?.zoneConfig?.validationDuration ?? 3f;
        countdownUICoroutine = StartCoroutine(CountdownUI(duration));
    }

    private void OnValidationComplete()
    {
        UpdateStatus("Starting game!", readyColor);

        if (progressSlider != null)
        {
            progressSlider.value = 1f;
            progressFill.color = readyColor;
        }

        if (countdownText != null)
        {
            countdownText.text = "GO!";
            countdownText.transform.DOPunchScale(Vector3.one * 0.5f, 0.5f);
        }
    }

    private void OnValidationCancelled()
    {
        UpdateStatus("Validation cancelled", waitingColor);

        if (progressSlider != null)
        {
            progressSlider.gameObject.SetActive(false);
        }

        if (countdownText != null)
        {
            countdownText.gameObject.SetActive(false);
        }

        if (countdownUICoroutine != null)
        {
            StopCoroutine(countdownUICoroutine);
        }
    }

    private System.Collections.IEnumerator CountdownUI(float duration)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / duration;

            // Update progress bar
            if (progressSlider != null)
            {
                progressSlider.value = progress;

                if (progressFill != null)
                {
                    progressFill.color = Color.Lerp(validatingColor, readyColor, progress);
                }
            }

            // Update countdown text
            if (countdownText != null)
            {
                float timeRemaining = duration - elapsed;
                countdownText.text = $"{timeRemaining:F1}s";

                // Make text pulse when time is running out
                if (timeRemaining <= 1f)
                {
                    float pulseScale = 1f + Mathf.Sin(Time.time * 10f) * 0.2f;
                    countdownText.transform.localScale = Vector3.one * pulseScale;
                }
            }

            yield return null;
        }
    }

    void OnDestroy()
    {
        if (validationZone != null)
        {
            validationZone.onPlayerCountChanged.RemoveListener(UpdatePlayerCount);
            validationZone.onValidationStart.RemoveListener(OnValidationStart);
            validationZone.onValidationComplete.RemoveListener(OnValidationComplete);
            validationZone.onValidationCancelled.RemoveListener(OnValidationCancelled);
        }

        if (countdownUICoroutine != null)
        {
            StopCoroutine(countdownUICoroutine);
        }
    }
}