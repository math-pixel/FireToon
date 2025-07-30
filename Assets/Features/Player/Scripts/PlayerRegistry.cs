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
        
        Vector3 spawnPos = gameConfig != null ? gameConfig.spawnPosition : 
                          (spawner != null ? spawner.transform.position : Vector3.zero);
        playerGO.transform.position = spawnPos;
        
        DontDestroyOnLoad(input.gameObject);
        currentNumberOfPlayers++;
    }

    public void Clear()
    {
        foreach (var player in RegisteredPlayers)
            if (player != null) Destroy(player.gameObject);

        RegisteredPlayers.Clear();
        currentNumberOfPlayers = 0;
        
        Debug.Log("PlayerRegistry cleared");
    }
    
    private void UpdatePlayerCountText()
    {
        if (playerCountText != null)
            playerCountText.text = playerCount.ToString();
    }
}
