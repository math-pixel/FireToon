using UnityEngine;
using UnityEngine.InputSystem;

public class LobbyManager : MonoBehaviour
{
    [Header("Configuration")]
    public LobbyConfig lobbyConfig;
    public GameStateConfig gameStateConfig;
    
    [Header("References")]
    public Timer timer;
    public PlayerRegistry playerRegistry;
    
    [Header("UI References")]
    public GameObject spawner;
    public TMPro.TMP_Text playerCountText;
    
    private bool timerStarted = false;

    private void Start()
    {
        // Use assigned reference or find PlayerRegistry
        if (playerRegistry == null)
        {
            playerRegistry = PlayerRegistry.Instance;
        }
        
        // Update PlayerRegistry UI references for this scene
        if (playerRegistry != null)
        {
            playerRegistry.UpdateUIReferences(spawner, playerCountText);
        }
        
        // Register with GameManager
        if (GameManager.Instance != null)
        {
            GameManager.Instance.lobbyManager = this;
            Debug.Log("LobbyManager registered with GameManager");
        }
    }

    private void Update()
    {
        CheckPlayersReady();
        CheckForceStart();
    }
    
    private void CheckPlayersReady()
    {
        if (playerRegistry == null || lobbyConfig == null) return;
        
        if (playerRegistry.IsAllPlayersRegistered() && lobbyConfig.autoStartWhenReady)
        {
            if (!timerStarted)
            {
                StartCountdown();
            }
        }
    }
    
    private void CheckForceStart()
    {
        if (lobbyConfig == null || !lobbyConfig.enableForceStart) return;
        
        if (Input.GetKeyDown(lobbyConfig.forceStartKey))
        {
            ChangeScene();
        }
    }
    
    private void StartCountdown()
    {
        if (timer == null || lobbyConfig == null) return;
        
        timerStarted = true;
        timer.StartCountdown(lobbyConfig.countdownDuration, ChangeScene);
        Debug.Log($"Countdown started: {lobbyConfig.countdownDuration} seconds");
    }

    private void ChangeScene()
    {
        if (GameManager.Instance == null) return;
        
        // ⭐ CORRECTION : Utiliser RequestStateChange
        GameManager.GameState nextState = GameManager.GameState.Playing;
        
        if (gameStateConfig != null)
        {
            nextState = gameStateConfig.GetNextState(GameManager.GameState.Lobby);
        }
        
        Debug.Log($"Requesting transition to {nextState}");
        GameManager.Instance.RequestStateChange(nextState);
    }

    private void OnPlayerJoined(PlayerInput playerInput)
    {
        Debug.Log("New Player joined");
        
        if (GameManager.Instance?.currentState == GameManager.GameState.Lobby)
        {
            playerRegistry?.RegisterPlayer(playerInput, playerInput.playerIndex);
        }
    }
    
    public void ForceStart()
    {
        ChangeScene();
    }
    
    public void ResetTimer()
    {
        timerStarted = false;
        timer?.StopCountdown();
        Debug.Log("Timer reset");
    }
    
    // Called by PlayerRegistry when player count changes
    public void UpdatePlayerCount()
    {
        // Reset timer when player count changes
        if (timerStarted && playerRegistry != null && !playerRegistry.IsAllPlayersRegistered())
        {
            ResetTimer();
        }
    }
    
    // Cleanup when leaving lobby
    void OnDestroy()
    {
        // Unregister from GameManager
        if (GameManager.Instance != null && GameManager.Instance.lobbyManager == this)
        {
            GameManager.Instance.lobbyManager = null;
        }
    }
}
