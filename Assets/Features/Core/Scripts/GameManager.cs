using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Configuration")] 
    public GameStateConfig gameStateConfig;

    public enum GameState
    {
        Start,
        Lobby,
        Menu,
        Playing,
        ScoreBoard,
        Paused,
        GameOver
    }

    public GameState currentState { get; private set; }
    public LobbyManager lobbyManager;

    public List<PlayerInput> LeaderboardPlayers = new List<PlayerInput>();

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        
        // ⭐ CORRECTION : Détecter la scène actuelle au lieu de forcer Start
        InitializeFromCurrentScene();
    }
    
    private void InitializeFromCurrentScene()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        
        // Détecter l'état basé sur la scène actuelle
        switch (currentSceneName)
        {
            case "MainMenu":
                currentState = GameState.Start; // Mais ne pas déclencher de transition
                Debug.Log("GameManager initialized in MainMenu");
                break;
            case "LobbyScene":
                currentState = GameState.Lobby;
                Debug.Log("GameManager initialized in Lobby");
                break;
            case "InGame":
                currentState = GameState.Playing;
                Debug.Log("GameManager initialized in Game");
                break;
            case "EndingScene":
                currentState = GameState.ScoreBoard;
                Debug.Log("GameManager initialized in Ending");
                break;
            default:
                currentState = GameState.Start;
                Debug.Log($"GameManager initialized in unknown scene: {currentSceneName}");
                break;
        }
    }

    // ⭐ NOUVELLE MÉTHODE : Pour déclencher des transitions explicites
    public void RequestStateChange(GameState newState)
    {
        Debug.Log($"State change requested: {currentState} → {newState}");
        UpdateState(newState);
    }

    public void PlayerDead(PlayerInput player)
    {
        LeaderboardPlayers.Insert(0, player);

        if (LeaderboardPlayers.Count == PlayerRegistry.Instance.RegisteredPlayers.Count - 1)
        {
            PlayerInput lastSurvivor = PlayerRegistry.Instance.RegisteredPlayers
                .First(p => !LeaderboardPlayers.Contains(p));

            LeaderboardPlayers.Insert(0, lastSurvivor);

            if (gameStateConfig != null && gameStateConfig.autoRestartOnGameOver)
            {
                StartCoroutine(DelayedGameOver());
            }
            else
            {
                GameOver();
            }
        }
    }

    private IEnumerator DelayedGameOver()
    {
        float delay = gameStateConfig != null ? gameStateConfig.gameOverDelay : 3f;
        yield return new WaitForSeconds(delay);
        GameOver();
    }

    public void GameOver()
    {
        Debug.Log("Game Over");
        RequestStateChange(GameState.GameOver);
    }

    public void RestartGame()
    {
        LeaderboardPlayers = new List<PlayerInput>();
        
        // Clear PlayerRegistry
        if (PlayerRegistry.Instance != null)
        {
            PlayerRegistry.Instance.Clear();
        }
        
        // Destroy LobbyManager if it exists
        if (lobbyManager != null)
        {
            Destroy(lobbyManager.gameObject);
            lobbyManager = null;
        }
        
        RequestStateChange(GameState.Start);
    }

    public void UpdateState(GameState newState)
    {
        GameState previousState = currentState;
        currentState = newState;
        
        Debug.Log($"State updated: {previousState} → {currentState}");

        if (gameStateConfig != null && gameStateConfig.autoTransitionScenes)
        {
            string sceneName = gameStateConfig.GetSceneName(currentState);
            if (!string.IsNullOrEmpty(sceneName))
            {
                // ⭐ VÉRIFICATION : Ne pas recharger la même scène
                string currentSceneName = SceneManager.GetActiveScene().name;
                if (currentSceneName == sceneName)
                {
                    Debug.Log($"Already in scene {sceneName}, skipping transition");
                    return;
                }
                
                float delay = gameStateConfig.GetTransitionDelay(currentState);
                if (delay > 0)
                {
                    StartCoroutine(DelayedSceneChange(sceneName, delay));
                }
                else
                {
                    ChangeScene(sceneName);
                }
                return;
            }
        }

        // Fallback to hardcoded scene names
        HandleStateTransition(newState);
    }

    private void HandleStateTransition(GameState newState)
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        
        switch (newState)
        {
            case GameState.Start:
                if (currentSceneName != "MainMenu")
                    ChangeScene("MainMenu");
                break;
            case GameState.Lobby:
                if (currentSceneName != "LobbyScene")
                    ChangeScene("LobbyScene");
                break;
            case GameState.Playing:
                if (currentSceneName != "InGame")
                    ChangeScene("InGame");
                break;
            case GameState.GameOver:
                RequestStateChange(GameState.ScoreBoard);
                break;
            case GameState.ScoreBoard:
                if (currentSceneName != "EndingScene")
                    ChangeScene("EndingScene");
                break;
            default:
                Debug.LogError($"Unhandled game state: {newState}");
                break;
        }
    }

    // Méthode mise à jour pour accepter le delay en paramètre
    private IEnumerator DelayedSceneChange(string sceneName, float delay)
    {
        yield return new WaitForSeconds(delay);
        ChangeScene(sceneName);
    }

    // Garde l'ancienne méthode pour compatibilité
    private IEnumerator DelayedSceneChange(string sceneName)
    {
        float delay = gameStateConfig != null ? gameStateConfig.defaultTransitionDelay : 0.5f;
        yield return new WaitForSeconds(delay);
        ChangeScene(sceneName);
    }

    public void ChangeScene(string sceneName)
    {
        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogError("Scene name is null or empty");
            return;
        }

        string currentSceneName = SceneManager.GetActiveScene().name;
        if (currentSceneName == sceneName)
        {
            Debug.Log($"Already in scene {sceneName}, skipping load");
            return;
        }

        // Log de transition si activé
        if (gameStateConfig != null && gameStateConfig.logStateTransitions)
        {
            Debug.Log($"Transitioning from {currentState} to scene: {sceneName}");
        }

        // Use CustomSceneManager if available for better transitions
        if (CustomSceneManager.Instance != null)
        {
            CustomSceneManager.Instance.LoadScene(sceneName);
        }
        else
        {
            // Fallback to direct scene loading
            SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
        }
    }

    public void PauseGame()
    {
        if (currentState == GameState.Playing)
        {
            RequestStateChange(GameState.Paused);
            Time.timeScale = 0f;
        }
    }

    public void ResumeGame()
    {
        if (currentState == GameState.Paused)
        {
            RequestStateChange(GameState.Playing);
            Time.timeScale = 1f;
        }
    }

    // Méthodes utilitaires supplémentaires
    public bool CanTransitionTo(GameState targetState)
    {
        if (gameStateConfig == null) return true;
        return gameStateConfig.HasSceneMapping(targetState);
    }

    public void ForceStateChange(GameState newState)
    {
        if (gameStateConfig != null && gameStateConfig.logStateTransitions)
        {
            Debug.Log($"Force changing state from {currentState} to {newState}");
        }

        currentState = newState;
    }

    public GameState GetNextState()
    {
        if (gameStateConfig != null)
        {
            return gameStateConfig.GetNextState(currentState);
        }

        // Fallback logic
        switch (currentState)
        {
            case GameState.Start: return GameState.Lobby;
            case GameState.Lobby: return GameState.Playing;
            case GameState.Playing: return GameState.GameOver;
            case GameState.GameOver: return GameState.ScoreBoard;
            case GameState.ScoreBoard: return GameState.Start;
            default: return currentState;
        }
    }

    // Method to ensure PlayerRegistry exists
    public void EnsurePlayerRegistry()
    {
        if (PlayerRegistry.Instance == null)
        {
            Debug.LogWarning("PlayerRegistry not found! Creating new instance...");
            
            GameObject playerRegistryGO = new GameObject("PlayerRegistry");
            playerRegistryGO.AddComponent<PlayerRegistry>();
            DontDestroyOnLoad(playerRegistryGO);
        }
    }
}
