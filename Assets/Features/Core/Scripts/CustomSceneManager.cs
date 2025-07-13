using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CustomSceneManager : MonoBehaviour
{
    public static CustomSceneManager Instance;

    [Header("Configuration")] public SceneTransitionConfig transitionConfig;

    [Header("UI References")] public Image fadeImage;
    public GameObject loadingScreen;
    public TextMeshProUGUI loadingText;

    private AudioSource audioSource;
    private bool isTransitioning = false;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
    }

    public void LoadScene(string sceneName, Action onComplete = null)
    {
        if (isTransitioning)
        {
            Debug.LogWarning($"Already transitioning, ignoring request to load {sceneName}");
            return;
        }

        string currentSceneName = SceneManager.GetActiveScene().name;
        if (currentSceneName == sceneName)
        {
            Debug.LogWarning($"Already in scene {sceneName}, skipping load");
            onComplete?.Invoke();
            return;
        }

        Debug.Log($"Loading scene: {currentSceneName} → {sceneName}");
        StartCoroutine(LoadSceneCoroutine(sceneName, onComplete));
    }

    private IEnumerator LoadSceneCoroutine(string sceneName, Action onComplete)
    {
        isTransitioning = true;

        // Play transition sound
        PlayTransitionSound();

        // Fade out
        yield return StartCoroutine(FadeOut());

        // Show loading screen
        if (transitionConfig != null && transitionConfig.showLoadingScreen && loadingScreen != null)
        {
            loadingScreen.SetActive(true);
            if (loadingText != null)
                loadingText.text = transitionConfig.loadingText;
        }

        // Start loading
        float startTime = Time.time;
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        asyncLoad.allowSceneActivation = false;

        // Wait for loading to complete
        while (!asyncLoad.isDone)
        {
            if (asyncLoad.progress >= 0.9f)
            {
                // Ensure minimum loading time
                float loadingTime = Time.time - startTime;
                float minTime = transitionConfig != null ? transitionConfig.minimumLoadingTime : 1f;

                if (loadingTime >= minTime)
                {
                    asyncLoad.allowSceneActivation = true;
                }
            }

            yield return null;
        }

        // Unload unused assets if configured
        if (transitionConfig != null && transitionConfig.unloadUnusedAssets)
        {
            yield return Resources.UnloadUnusedAssets();
        }

        // Hide loading screen
        if (loadingScreen != null)
            loadingScreen.SetActive(false);

        // Fade in
        yield return StartCoroutine(FadeIn());

        isTransitioning = false;
        onComplete?.Invoke();

        Debug.Log($"Scene transition completed: {sceneName}");
    }


    private IEnumerator FadeOut()
    {
        if (fadeImage == null || transitionConfig == null) yield break;

        fadeImage.gameObject.SetActive(true);
        fadeImage.color = new Color(transitionConfig.fadeColor.r, transitionConfig.fadeColor.g,
            transitionConfig.fadeColor.b, 0);

        float elapsed = 0;
        while (elapsed < transitionConfig.fadeOutDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float alpha = elapsed / transitionConfig.fadeOutDuration;
            fadeImage.color = new Color(transitionConfig.fadeColor.r, transitionConfig.fadeColor.g,
                transitionConfig.fadeColor.b, alpha);
            yield return null;
        }

        fadeImage.color = transitionConfig.fadeColor;
    }

    private IEnumerator FadeIn()
    {
        if (fadeImage == null || transitionConfig == null) yield break;

        float elapsed = 0;
        while (elapsed < transitionConfig.fadeInDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float alpha = 1 - (elapsed / transitionConfig.fadeInDuration);
            fadeImage.color = new Color(transitionConfig.fadeColor.r, transitionConfig.fadeColor.g,
                transitionConfig.fadeColor.b, alpha);
            yield return null;
        }

        fadeImage.color = new Color(transitionConfig.fadeColor.r, transitionConfig.fadeColor.g,
            transitionConfig.fadeColor.b, 0);
        fadeImage.gameObject.SetActive(false);
    }

    private void PlayTransitionSound()
    {
        if (transitionConfig?.transitionSound != null && audioSource != null)
        {
            audioSource.clip = transitionConfig.transitionSound;
            audioSource.volume = transitionConfig.transitionVolume;
            audioSource.Play();
        }
    }

    public bool IsTransitioning()
    {
        return isTransitioning;
    }
}