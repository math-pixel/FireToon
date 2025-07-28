using UnityEngine;
using UnityEngine.InputSystem;

public class LobbyManager : MonoBehaviour
{
    [Header("Configuration")]
    public LobbyConfig lobbyConfig;
    public GameStateConfig gameStateConfig;
    
    [Header("References")]
    public PlayerRegistry playerRegistry;
    public ValidationZone validationZone;
    
    [Header("UI References")]
    public GameObject spawner;
    public TMPro.TMP_Text playerCountText;
    
    
    private void Start()
    {
        if (playerRegistry == null)
        {
            playerRegistry = PlayerRegistry.Instance;
        }
        
        if (playerRegistry != null)
        {
            playerRegistry.UpdateUIReferences(spawner, playerCountText);
        }
        
        if (validationZone != null)
        {
            validationZone.onValidationComplete.AddListener(OnValidationComplete);
        }
        
        if (GameManager.Instance != null)
        {
            GameManager.Instance.lobbyManager = this;
            Debug.Log("LobbyManager registered with GameManager");
        }
    }

    private void Update()
    {
        CheckForceStart();
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
    
    public void UpdatePlayerCount()
    {
    }
    
    void OnDestroy()
    {
        if (validationZone != null)
        {
            validationZone.onValidationComplete.RemoveListener(OnValidationComplete);
        }
        
        if (GameManager.Instance != null && GameManager.Instance.lobbyManager == this)
        {
            GameManager.Instance.lobbyManager = null;
        }
    }
}
