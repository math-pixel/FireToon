using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;
using System;

public class MenuManager : MonoBehaviour
{
    [Header("Animation")]
    public UIAnimator uiAnimator; // ⭐ Garde l'UIAnimator
    
    [Header("Callbacks")]
    public UnityEvent onPlayButtonClicked;
    public UnityEvent onSettingsButtonClicked;
    public UnityEvent onMenuAnimationComplete;
    
    [Header("Settings")]
    public Action<bool> onSettingsToggle;
    public GameObject settingsPanel;
    public SettingsManager settingsManager;
    
    private bool settingsOpen = false;
    
    void Start()
    {
        Debug.Log("MenuManager Start() called");
        
        // Setup default callbacks if none assigned
        if (onPlayButtonClicked.GetPersistentEventCount() == 0)
        {
            onPlayButtonClicked.AddListener(StartGame);
            Debug.Log("Added StartGame callback to onPlayButtonClicked");
        }
        
        if (onSettingsButtonClicked.GetPersistentEventCount() == 0)
        {
            onSettingsButtonClicked.AddListener(ToggleSettings);
            Debug.Log("Added ToggleSettings callback to onSettingsButtonClicked");
        }
        
        // Hide settings panel initially
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
        }
        
        // ⭐ Start animations via UIAnimator
        if (uiAnimator != null)
        {
            Debug.Log("Starting UIAnimator animations");
            uiAnimator.PlayAnimations();
        }
        else
        {
            Debug.LogWarning("UIAnimator reference is null!");
        }
    }
    
    // Public methods for UI Button events
    public void OnPlayButtonClick()
    {
        Debug.Log("OnPlayButtonClick() called!");
        onPlayButtonClicked?.Invoke();
    }
    
    public void OnSettingsButtonClick()
    {
        Debug.Log("OnSettingsButtonClick() called!");
        onSettingsButtonClicked?.Invoke();
    }
    
    // Default callback implementations
    private void StartGame()
    {
        Debug.Log("StartGame() called!");
        
        // Stop current animations
        if (uiAnimator != null)
        {
            uiAnimator.StopAllAnimations();
        }
        
        // Transition to lobby
        if (GameManager.Instance != null)
        {
            Debug.Log("Requesting state change to Lobby");
            GameManager.Instance.RequestStateChange(GameManager.GameState.Lobby);
        }
        else if (CustomSceneManager.Instance != null)
        {
            Debug.Log("Using CustomSceneManager to load LobbyScene");
            CustomSceneManager.Instance.LoadScene("LobbyScene", () => {
                Debug.Log("Transitioned to Lobby");
            });
        }
        else
        {
            Debug.LogError("No GameManager or CustomSceneManager found!");
        }
    }
    
    private void ToggleSettings()
    {
        Debug.Log("ToggleSettings() called!");
        settingsOpen = !settingsOpen;
        
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(settingsOpen);
            
            // Initialize settings manager if opening for first time
            if (settingsOpen && settingsManager != null)
            {
                settingsManager.LoadSettings();
            }
            
            // Animate settings panel
            if (settingsOpen)
            {
                AnimateSettingsIn();
            }
            else
            {
                AnimateSettingsOut();
            }
        }
        
        // Notify external systems
        onSettingsToggle?.Invoke(settingsOpen);
    }
    
    private void AnimateSettingsIn()
    {
        if (settingsPanel == null) return;
        
        RectTransform settingsRect = settingsPanel.GetComponent<RectTransform>();
        CanvasGroup settingsCanvas = settingsPanel.GetComponent<CanvasGroup>();
        
        if (settingsCanvas == null)
        {
            settingsCanvas = settingsPanel.AddComponent<CanvasGroup>();
        }
        
        // Start from invisible and scaled down
        settingsCanvas.alpha = 0f;
        settingsRect.localScale = Vector3.zero;
        
        // Animate in
        settingsCanvas.DOFade(1f, 0.3f);
        settingsRect.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack);
    }
    
    private void AnimateSettingsOut()
    {
        if (settingsPanel == null) return;
        
        RectTransform settingsRect = settingsPanel.GetComponent<RectTransform>();
        CanvasGroup settingsCanvas = settingsPanel.GetComponent<CanvasGroup>();
        
        if (settingsCanvas != null)
        {
            settingsCanvas.DOFade(0f, 0.2f);
        }
        
        settingsRect.DOScale(Vector3.zero, 0.2f)
            .SetEase(Ease.InBack)
            .OnComplete(() => {
                settingsPanel.SetActive(false);
            });
    }
    
    // Public methods for external control
    public void SetPlayCallback(UnityAction callback)
    {
        onPlayButtonClicked.RemoveAllListeners();
        onPlayButtonClicked.AddListener(callback);
    }
    
    public void SetSettingsCallback(UnityAction callback)
    {
        onSettingsButtonClicked.RemoveAllListeners();
        onSettingsButtonClicked.AddListener(callback);
    }
    
    public void SetSettingsToggleCallback(Action<bool> callback)
    {
        onSettingsToggle = callback;
    }
    
    public void PlayCustomAnimation(string animationName)
    {
        if (uiAnimator != null)
        {
            uiAnimator.PlayAnimations();
        }
    }
    
    public void ResetMenu()
    {
        if (uiAnimator != null)
        {
            uiAnimator.ResetToOriginal();
        }
        
        if (settingsPanel != null && settingsOpen)
        {
            ToggleSettings();
        }
    }
    
    void OnDestroy()
    {
        // Cleanup DOTween animations
        settingsPanel?.GetComponent<RectTransform>()?.DOKill();
        settingsPanel?.GetComponent<CanvasGroup>()?.DOKill();
    }
}
