using UnityEngine;
using TMPro;
using System;
using System.Collections;

public class Timer : MonoBehaviour
{
    [Header("Durée du compte à rebours en secondes")]
    public int duration = 10;

    [Header("Référence au texte TMP où afficher le timer")]
    public TMP_Text timerText;

    // Callback appelée quand le timer atteint 0
    private Action onTimerFinished;

    /// <summary>
    /// Lance le compte à rebours avec une durée et une callback.
    /// </summary>
    /// <param name="seconds">Durée du timer</param>
    /// <param name="callback">Méthode à appeler à la fin</param>
    public void StartCountdown(int seconds, Action callback)
    {
        duration = seconds;
        onTimerFinished = callback;
        StopAllCoroutines(); // Permet de redémarrer proprement
        StartCoroutine(CountdownCoroutine());
    }

    // Coroutine de décompte
    private IEnumerator CountdownCoroutine()
    {
        int remaining = duration;

        while (remaining > 0)
        {
            if (timerText != null)
                timerText.text = remaining.ToString();

            yield return new WaitForSecondsRealtime(1f);
            remaining--;
        }

        if (timerText != null)
            timerText.text = "0";

        // Appelle la fonction finale si elle existe
        onTimerFinished?.Invoke();
    }
}