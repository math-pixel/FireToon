using UnityEngine;
using TMPro;
using System;
using System.Collections;

public class Timer : MonoBehaviour
{
    [Header("Configuration")]
    public TimerConfig timerConfig;
    
    [Header("References")]
    public TMP_Text timerText;
    
    private Action onTimerFinished;
    private Coroutine currentCountdown;
    private float currentDuration;
    private bool isPaused = false;
    private float pausedTimeRemaining;

    // Overload pour accepter float (cohérent avec LobbyConfig)
    public void StartCountdown(float seconds, Action callback)
    {
        currentDuration = seconds;
        onTimerFinished = callback;
        StopCountdown();
        isPaused = false;
        currentCountdown = StartCoroutine(CountdownCoroutine());
    }
    
    // Garde la compatibilité avec int
    public void StartCountdown(int seconds, Action callback)
    {
        StartCountdown((float)seconds, callback);
    }
    
    public void StartCountdown(Action callback)
    {
        if (timerConfig != null)
        {
            StartCountdown((float)timerConfig.defaultDuration, callback);
        }
    }
    
    public void StopCountdown()
    {
        if (currentCountdown != null)
        {
            StopCoroutine(currentCountdown);
            currentCountdown = null;
        }
        isPaused = false;
    }

    private IEnumerator CountdownCoroutine()
    {
        float remaining = currentDuration;

        while (remaining > 0 && !isPaused)
        {
            UpdateTimerDisplay(remaining);
            
            if (timerConfig != null && timerConfig.useRealTime)
            {
                yield return new WaitForSecondsRealtime(0.1f);
                remaining -= 0.1f;
            }
            else
            {
                yield return new WaitForSeconds(0.1f);
                remaining -= 0.1f;
            }
        }

        if (!isPaused)
        {
            UpdateTimerDisplay(0);
            onTimerFinished?.Invoke();
            currentCountdown = null;
        }
        else
        {
            pausedTimeRemaining = remaining;
        }
    }
    
    private void UpdateTimerDisplay(float remaining)
    {
        if (timerText == null) return;
        
        // Use config if available, otherwise show countdown
        bool showCountdown = timerConfig?.showCountdown ?? true;
        if (!showCountdown) return;
        
        string displayText;
        if (remaining <= 0)
        {
            displayText = timerConfig?.finishedText ?? "0";
        }
        else
        {
            string format = timerConfig?.countdownFormat ?? "{0}";
            // Round to nearest integer for display
            displayText = string.Format(format, Mathf.Ceil(remaining));
        }
            
        timerText.text = displayText;
        
        // Change color based on urgency
        if (timerConfig != null)
        {
            if (remaining <= timerConfig.urgentThreshold && remaining > 0)
            {
                timerText.color = timerConfig.urgentColor;
            }
            else
            {
                timerText.color = timerConfig.normalColor;
            }
        }
    }
    
    public bool IsRunning()
    {
        return currentCountdown != null && !isPaused;
    }
    
    public bool IsPaused()
    {
        return isPaused;
    }
    
    public void PauseCountdown()
    {
        if (currentCountdown != null && !isPaused)
        {
            isPaused = true;
        }
    }
    
    public void ResumeCountdown()
    {
        if (isPaused && currentCountdown == null)
        {
            isPaused = false;
            currentDuration = pausedTimeRemaining;
            currentCountdown = StartCoroutine(CountdownCoroutine());
        }
    }
    
    public float GetTimeRemaining()
    {
        return isPaused ? pausedTimeRemaining : currentDuration;
    }
    
    public void AddTime(float seconds)
    {
        if (IsRunning())
        {
            currentDuration += seconds;
        }
        else if (isPaused)
        {
            pausedTimeRemaining += seconds;
        }
    }
    
    public void SetTimeRemaining(float seconds)
    {
        if (IsRunning())
        {
            StopCountdown();
            StartCountdown(seconds, onTimerFinished);
        }
        else if (isPaused)
        {
            pausedTimeRemaining = seconds;
        }
    }
}
