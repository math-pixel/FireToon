using UnityEngine;

[CreateAssetMenu(fileName = "SceneTransitionConfig", menuName = "Game/Scene Transition ScriptableObjects")]
public class SceneTransitionConfig : ScriptableObject
{
    [Header("Transition Settings")]
    public float fadeInDuration = 0.5f;
    public float fadeOutDuration = 0.5f;
    public Color fadeColor = Color.black;
    
    [Header("Loading Settings")]
    public bool showLoadingScreen = true;
    public float minimumLoadingTime = 1f;
    public string loadingText = "Loading...";
    
    [Header("Audio Settings")]
    public AudioClip transitionSound;
    public float transitionVolume = 1f;
    
    [Header("Performance")]
    public bool preloadScenes = false;
    public bool unloadUnusedAssets = true;
}