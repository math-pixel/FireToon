using UnityEngine;
using UnityEngine.InputSystem;

public class LobbyManager : MonoBehaviour
{
    [Header("Configuration")]
    public LobbyConfig lobbyConfig;
    public GameStateConfig gameStateConfig;
    
    [Header("References")]
    public PlayerRegistry playerRegistry;
    public ValidationZone validationZone; // ⭐ Nouvelle référence
    
    [Header("UI References")]
    public GameObject spawner;
    public TMPro.TMP_Text playerCountText;
    
    // Supprimer les références Timer car on utilise maintenant la zone
    
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
        
        // Setup validation zone
        if (validationZone != null)
        {
            validationZone.onValidationComplete.AddListener(OnValidationComplete);
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
        CheckForceStart();
        // Supprimer CheckPlayersReady() car la zone gère maintenant la validation
    }
    
    private void CheckForceStart()
    {
        if (lobbyConfig == null || !lobbyConfig.enableForceStart) return;
        
        if (Input.GetKeyDown(lobbyConfig.forceStartKey))
        {
            ForceStart();
        }
    }
    
    private void OnValidationComplete()
    {
        Debug.Log("Validation zone completed - starting game");
        ChangeScene();
    }

    private void ChangeScene()
    {
        if (GameManager.Instance == null) return;
        
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
        if (validationZone != null)
        {
            validationZone.ForceCompleteValidation();
        }
        else
        {
            ChangeScene();
        }
    }
    
    // Called by PlayerRegistry when player count changes
    public void UpdatePlayerCount()
    {
        // La zone de validation gère maintenant la logique
        // Optionnel : mettre à jour d'autres UI si nécessaire
    }
    
    void OnDestroy()
    {
        // Cleanup validation zone events
        if (validationZone != null)
        {
            validationZone.onValidationComplete.RemoveListener(OnValidationComplete);
        }
        
        // Unregister from GameManager
        if (GameManager.Instance != null && GameManager.Instance.lobbyManager == this)
        {
            GameManager.Instance.lobbyManager = null;
        }
    }
}
