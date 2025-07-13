using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "GameStateConfig", menuName = "Game/Game State ScriptableObjects")]
public class GameStateConfig : ScriptableObject
{
    [System.Serializable]
    public class SceneMapping
    {
        public GameManager.GameState state;
        public string sceneName;
        public float transitionDelay = 0f;
        public bool useCustomTransition = false;
    }
    
    public List<SceneMapping> sceneMappings = new List<SceneMapping>
    {
        new SceneMapping { state = GameManager.GameState.Start, sceneName = "MainMenu" },
        new SceneMapping { state = GameManager.GameState.Lobby, sceneName = "LobbyScene" },
        new SceneMapping { state = GameManager.GameState.Playing, sceneName = "InGame" },
        new SceneMapping { state = GameManager.GameState.ScoreBoard, sceneName = "EndingScene" }
    };
    
    [Header("Transition Settings")]
    public bool autoTransitionScenes = true;
    public float defaultTransitionDelay = 0.5f;
    public bool showTransitionEffects = true;
    
    [Header("Game Flow")]
    public bool autoRestartOnGameOver = false;
    public float gameOverDelay = 3f;
    public bool skipGameOverState = false;
    
    [Header("State Validation")]
    public bool validateSceneNames = true;
    public bool logStateTransitions = true;
    
    public string GetSceneName(GameManager.GameState state)
    {
        foreach (var mapping in sceneMappings)
        {
            if (mapping.state == state)
                return mapping.sceneName;
        }
        
        if (logStateTransitions)
            Debug.LogWarning($"No scene mapping found for state: {state}");
            
        return "";
    }
    
    public float GetTransitionDelay(GameManager.GameState state)
    {
        foreach (var mapping in sceneMappings)
        {
            if (mapping.state == state)
                return mapping.transitionDelay > 0 ? mapping.transitionDelay : defaultTransitionDelay;
        }
        return defaultTransitionDelay;
    }
    
    public GameManager.GameState GetNextState(GameManager.GameState currentState)
    {
        switch (currentState)
        {
            case GameManager.GameState.Start:
                return GameManager.GameState.Lobby;
            case GameManager.GameState.Lobby:
                return GameManager.GameState.Playing;
            case GameManager.GameState.Playing:
                return skipGameOverState ? GameManager.GameState.ScoreBoard : GameManager.GameState.GameOver;
            case GameManager.GameState.GameOver:
                return GameManager.GameState.ScoreBoard;
            case GameManager.GameState.ScoreBoard:
                return autoRestartOnGameOver ? GameManager.GameState.Start : GameManager.GameState.Lobby;
            default:
                return currentState;
        }
    }
    
    public bool HasSceneMapping(GameManager.GameState state)
    {
        return !string.IsNullOrEmpty(GetSceneName(state));
    }
    
    [ContextMenu("Validate Scene Names")]
    public void ValidateSceneNames()
    {
        if (!validateSceneNames) return;
        
        foreach (var mapping in sceneMappings)
        {
            if (string.IsNullOrEmpty(mapping.sceneName))
            {
                Debug.LogError($"Empty scene name for state: {mapping.state}");
            }
        }
    }
}
