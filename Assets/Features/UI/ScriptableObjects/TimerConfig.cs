using UnityEngine;

[CreateAssetMenu(fileName = "TimerConfig", menuName = "Game/Timer ScriptableObjects")]
public class TimerConfig : ScriptableObject
{
    [Header("Timer Settings")]
    public int defaultDuration = 10;
    public bool useRealTime = true;
    public string finishedText = "0";
    public float updateInterval = 0.1f; // Pour un affichage plus fluide
    
    [Header("Display Settings")]
    public bool showCountdown = true;
    public string countdownFormat = "{0}";
    public bool showDecimals = false;
    public string decimalFormat = "{0:F1}";
    
    [Header("Visual Settings")]
    public Color normalColor = Color.white;
    public Color urgentColor = Color.red;
    public int urgentThreshold = 3;
    
    [Header("Animation Settings")]
    public bool animateUrgentText = true;
    public float urgentPulseSpeed = 2f;
    
    public string GetFormattedTime(float timeRemaining)
    {
        if (timeRemaining <= 0)
            return finishedText;
            
        if (showDecimals)
            return string.Format(decimalFormat, timeRemaining);
        else
            return string.Format(countdownFormat, Mathf.Ceil(timeRemaining));
    }
}