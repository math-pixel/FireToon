using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;
using TMPro;

public class ValidationZone : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private ZoneValidationConfig zoneConfig;

    [Header("References")]
    [SerializeField] private Renderer zoneRenderer;

    [SerializeField] private TextMeshPro counterText;

    [Header("Events")]
    public UnityEvent onValidationStart;

    public UnityEvent onValidationComplete;
    public UnityEvent onValidationCancelled;
    public UnityEvent<int, int> onPlayerCountChanged;

    private readonly HashSet<GameObject> playersInZone = new HashSet<GameObject>();
    private bool isValidating;
    private Coroutine validationCoroutine;
    private Material zoneMaterial;
    private Color originalColor;

    // Properties
    public int PlayersInZone => playersInZone.Count;
    public int RequiredPlayers => GetRequiredPlayerCount();
    public bool IsValidating => isValidating;

    #region Unity Lifecycle

    private void Start()
    {
        InitializeZone();
        onPlayerCountChanged.AddListener(UpdateCounterText);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (IsPlayer(other.gameObject))
        {
            AddPlayer(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (IsPlayer(other.gameObject))
        {
            RemovePlayer(other.gameObject);
        }
    }

    private void OnDestroy()
    {
        StopValidationCoroutine();
    }

    #endregion

    #region Initialization

    private void InitializeZone()
    {
        if (zoneRenderer != null)
        {
            zoneMaterial = zoneRenderer.material;
            originalColor = zoneConfig?.zoneNormalColor ?? Color.green;
            zoneMaterial.color = originalColor;
        }

        UpdateCounterText(PlayersInZone, RequiredPlayers);
        
        Debug.Log($"ValidationZone initialized. Required players: {RequiredPlayers}");
    }

    #endregion

    #region Player Management

    private bool IsPlayer(GameObject obj)
    {
        return obj.CompareTag("Player") || obj.GetComponent<UnityEngine.InputSystem.PlayerInput>() != null;
    }

    private void AddPlayer(GameObject player)
    {
        if (!playersInZone.Add(player)) return;

        Debug.Log($"Player {player.name} entered validation zone. ({PlayersInZone}/{RequiredPlayers})");
        onPlayerCountChanged?.Invoke(PlayersInZone, RequiredPlayers);
        CheckValidationConditions();
    }

    private void RemovePlayer(GameObject player)
    {
        if (!playersInZone.Remove(player)) return;

        Debug.Log($"Player {player.name} left validation zone. ({PlayersInZone}/{RequiredPlayers})");
        onPlayerCountChanged?.Invoke(PlayersInZone, RequiredPlayers);

        if (isValidating && !HasEnoughPlayers())
        {
            CancelValidation();
        }
    }

    private int GetRequiredPlayerCount()
    {
        if (zoneConfig == null) return 1;

        if (zoneConfig.requireAllPlayers && PlayerRegistry.Instance != null)
        {
            return PlayerRegistry.Instance.RegisteredPlayers.Count;
        }

        return zoneConfig.minimumPlayersRequired;
    }

    #endregion

    #region Validation Logic

    private bool HasEnoughPlayers() => PlayersInZone >= RequiredPlayers;

    private void CheckValidationConditions()
    {
        if (!isValidating && HasEnoughPlayers())
        {
            StartValidation();
        }
    }

    private void StartValidation()
    {
        if (isValidating) return;

        isValidating = true;
        float duration = zoneConfig?.validationDuration ?? 3f;

        Debug.Log($"Starting validation countdown ({duration} seconds)");
        ChangeZoneColor(zoneConfig?.zoneValidatingColor ?? Color.yellow);
        onValidationStart?.Invoke();

        validationCoroutine = StartCoroutine(ValidationCountdown(duration));
    }

    private void CancelValidation()
    {
        if (!isValidating) return;

        isValidating = false;
        Debug.Log("Validation cancelled - not enough players");

        StopValidationCoroutine();
        ChangeZoneColor(originalColor);
        onValidationCancelled?.Invoke();
    }

    private IEnumerator ValidationCountdown(float duration)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            if (!HasEnoughPlayers())
            {
                CancelValidation();
                yield break;
            }

            elapsed += Time.deltaTime;
            UpdateValidationProgress(elapsed / duration);
            yield return null;
        }

        CompleteValidation();
    }

    private void CompleteValidation()
    {
        isValidating = false;
        Debug.Log("Validation complete! Starting game...");

        ChangeZoneColor(zoneConfig?.zoneReadyColor ?? Color.blue);
        onValidationComplete?.Invoke();
        StartGameTransition();
    }

    #endregion

    #region Visual Effects

    private void UpdateValidationProgress(float progress)
    {
        if (zoneConfig && zoneMaterial)
        {
            Color currentColor = Color.Lerp(zoneConfig.zoneValidatingColor, zoneConfig.zoneReadyColor, progress);
            zoneMaterial.color = currentColor;
        }
    }

    private void ChangeZoneColor(Color targetColor)
    {
        if (zoneMaterial && zoneConfig)
        {
            float transitionSpeed = zoneConfig.colorTransitionSpeed;
            zoneMaterial.DOColor(targetColor, 1f / transitionSpeed);
        }
    }

    private void UpdateCounterText(int current, int required)
    {
        if (counterText != null)
        {
            counterText.text = $"{current}/{required}";
        }
    }

    #endregion

    #region Public Methods

    public void ForceCompleteValidation()
    {
        if (isValidating)
        {
            StopValidationCoroutine();
            CompleteValidation();
        }
    }

    public void ResetZone()
    {
        CancelValidation();
        playersInZone.Clear();
        onPlayerCountChanged?.Invoke(0, RequiredPlayers);
    }

    #endregion

    #region Private Helpers

    private void StopValidationCoroutine()
    {
        if (validationCoroutine != null)
        {
            StopCoroutine(validationCoroutine);
            validationCoroutine = null;
        }
    }

    private void StartGameTransition()
    {
        GameManager.Instance?.RequestStateChange(GameManager.GameState.Playing);
    }

    #endregion
}
