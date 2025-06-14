using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class PlayerRegistry : MonoBehaviour
{
    public static PlayerRegistry Instance;
    public List<PlayerInput> RegisteredPlayers = new List<PlayerInput>();

    public GameObject spawner;
    
    public int playerCount = 1;
    public TMP_Text playerCountText;
    
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
    }

    public void RegisterPlayer(PlayerInput input, int playerNumber)
    {
        RegisteredPlayers.Add(input);
        
        // change Name player
        // its auto instanciate
        GameObject playerGO = input.gameObject;
        playerGO.name = $"Player {playerNumber}";
        playerGO.transform.position = spawner.transform.position;
        
        // Change skin of player
        // GameObject Player = Instantiate(PlayerPrefab, input.gameObject.transform);
        // Player.transform.localScale = new Vector3(1, 1, 1) * 2.88f;
        // Player.transform.localPosition = new Vector3(0, -0.9f, 0);
        
        
        // save player across scene
        DontDestroyOnLoad(input.gameObject); // garde le joueur à travers les scènes
        currentNumberOfPlayers++;
        
    }

    public void Clear()
    {
        foreach (var player in RegisteredPlayers)
            Destroy(player.gameObject);

        RegisteredPlayers.Clear();
        currentNumberOfPlayers = 0;
    }

    public bool IsAllPlayersRegistered()
    {
        if (currentNumberOfPlayers == playerCount)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    
    public void AddPlayerCount()
    {
        if (playerCount + 1 <= 4)
        {
            playerCount++;
            playerCountText.text = playerCount.ToString();
        }
    }

    public void RemovePlayerCount()
    {
        if (playerCount - 1 >= 1)
        {
            playerCount--;
            playerCountText.text = playerCount.ToString();
        }
    }
}
