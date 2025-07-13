using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class PlayerRegistry : MonoBehaviour
{
    public static PlayerRegistry Instance;
    
    [Header("Configuration")]
    public GameConfig gameConfig;
    
    [Header("References")]
    public GameObject spawner;
    public TMP_Text playerCountText;
    
    public List<PlayerInput> RegisteredPlayers = new List<PlayerInput>();
    public int playerCount = 1;
    
    private int currentNumberOfPlayers = 0;
    
    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        
        // Initialize with config values
        if (gameConfig != null)
        {
            playerCount = gameConfig.minPlayers;
            UpdatePlayerCountText();
        }
    }

    public void RegisterPlayer(PlayerInput input, int playerNumber)
    {
        RegisteredPlayers.Add(input);
        
        GameObject playerGO = input.gameObject;
        playerGO.name = $"Player {playerNumber}";
        
        // Use config for positioning
        Vector3 spawnPos = gameConfig != null ? gameConfig.spawnPosition : 
                          (spawner != null ? spawner.transform.position : Vector3.zero);
        playerGO.transform.position = spawnPos;
        
        DontDestroyOnLoad(input.gameObject);
        currentNumberOfPlayers++;
        
        Debug.Log($"Player {playerNumber} registered. Total: {currentNumberOfPlayers}/{playerCount}");
    }

    public void Clear()
    {
        foreach (var player in RegisteredPlayers)
            if (player != null) Destroy(player.gameObject);

        RegisteredPlayers.Clear();
        currentNumberOfPlayers = 0;
        
        Debug.Log("PlayerRegistry cleared");
    }

    public bool IsAllPlayersRegistered()
    {
        return currentNumberOfPlayers == playerCount;
    }
    
    public void AddPlayerCount()
    {
        int maxPlayers = gameConfig != null ? gameConfig.maxPlayers : 4;
        if (playerCount + 1 <= maxPlayers)
        {
            playerCount++;
            UpdatePlayerCountText();
            
            // Notify LobbyManager if it exists
            NotifyLobbyManager();
        }
    }

    public void RemovePlayerCount()
    {
        int minPlayers = gameConfig != null ? gameConfig.minPlayers : 1;
        if (playerCount - 1 >= minPlayers)
        {
            playerCount--;
            UpdatePlayerCountText();
            
            // Notify LobbyManager if it exists
            NotifyLobbyManager();
        }
    }
    
    private void UpdatePlayerCountText()
    {
        if (playerCountText != null)
            playerCountText.text = playerCount.ToString();
    }
    
    private void NotifyLobbyManager()
    {
        // Notify LobbyManager about player count change
        if (GameManager.Instance?.lobbyManager != null)
        {
            GameManager.Instance.lobbyManager.UpdatePlayerCount();
        }
    }
    
    // Method to update UI references when changing scenes
    public void UpdateUIReferences(GameObject newSpawner, TMP_Text newPlayerCountText)
    {
        spawner = newSpawner;
        playerCountText = newPlayerCountText;
        UpdatePlayerCountText();
    }
}
