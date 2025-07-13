using UnityEngine;
using DG.Tweening;

public class EndingManager : MonoBehaviour
{
    [Header("Configuration")]
    public UIAnimationConfig animationConfig;
    
    [Header("UI References")]
    public RectTransform replayButton;
    public Vector3 buttonEndPosition;
    
    private Sequence buttonSequence;
    
    void Start()
    {
        AnimateReplayButton();
    }
    
    private void AnimateReplayButton()
    {
        if (replayButton == null) return;
        
        // Use config values or fallbacks
        float duration = animationConfig?.replayButtonDuration ?? 2f;
        float delay = animationConfig?.replayButtonDelay ?? 2f;
        Vector3 scale = animationConfig?.replayButtonScale ?? new Vector3(1.2f, 1.2f, 1.2f);
        float scaleDuration = animationConfig?.replayScaleDuration ?? 5f;
        Ease ease = animationConfig?.replayEase ?? Ease.OutBack;
        
        buttonSequence = DOTween.Sequence();
        
        buttonSequence.Append(replayButton.DOAnchorPos(buttonEndPosition, duration)
                .SetEase(ease)
                .SetDelay(delay))
            .OnComplete(() =>
            {
                replayButton.DOScale(scale, scaleDuration)
                    .SetLoops(-1, LoopType.Yoyo);
            });
    }
    
    public void RestartGame()
    {
        // Stop animations before restarting
        if (buttonSequence != null)
        {
            buttonSequence.Kill();
        }
        
        replayButton.DOKill();
        
        // ⭐ CORRECTION : Utiliser RequestStateChange
        if (GameManager.Instance != null)
        {
            GameManager.Instance.RestartGame(); // RestartGame() utilise déjà RequestStateChange
        }
        else
        {
            Debug.LogError("GameManager not found!");
        }
    }
    
    void OnDestroy()
    {
        // Clean up DOTween sequences
        if (buttonSequence != null)
        {
            buttonSequence.Kill();
        }
        replayButton?.DOKill();
    }
}
