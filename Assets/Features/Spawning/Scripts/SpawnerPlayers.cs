using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SpawnerPlayers : MonoBehaviour
{
    [Header("Configuration")]
    public SpawnConfig spawnConfig;
    
    [Header("Fallback Spawn Points")]
    public Transform[] fallbackSpawnPoints;
    
    private List<PlayerInput> _playerInputs;
    
    void Start()
    {
        GetPlayerInputs();
        SpawnPlayers(_playerInputs);
    }

    private void GetPlayerInputs()
    {
        if (GameManager.Instance.currentState == GameManager.GameState.Playing)
        {
            _playerInputs = PlayerRegistry.Instance.RegisteredPlayers;
        }
        else if (GameManager.Instance.currentState == GameManager.GameState.ScoreBoard)
        {
            _playerInputs = GameManager.Instance.LeaderboardPlayers;
        }
    }

    void SpawnPlayers(List<PlayerInput> players)
    {
        if (players == null) return;
        
        Transform[] spawnPoints = GetSpawnPoints();
        
        for (int i = 0; i < players.Count && i < spawnPoints.Length; i++)
        {
            var player = players[i];
            SpawnPlayer(player, spawnPoints[i], i);
        }
    }
    
    private Transform[] GetSpawnPoints()
    {
        if (spawnConfig != null && spawnConfig.spawnPoints != null && spawnConfig.spawnPoints.Length > 0)
        {
            return spawnConfig.spawnPoints;
        }
        return fallbackSpawnPoints;
    }
    
    private void SpawnPlayer(PlayerInput player, Transform spawnPoint, int playerIndex)
    {
        player.gameObject.SetActive(true);
        player.transform.position = spawnPoint.position;
        player.transform.rotation = spawnPoint.rotation;

        if (GameManager.Instance.currentState == GameManager.GameState.ScoreBoard)
        {
            SetupScoreboardPlayer(player, playerIndex);
        }
    }
    
    private void SetupScoreboardPlayer(PlayerInput player, int playerIndex)
    {
        PlayerMovement playerMovement = player.GetComponent<PlayerMovement>();
        if (playerMovement == null) return;
        
        // Disable movement if configured
        if (spawnConfig != null && spawnConfig.disableMovementOnScoreboard)
        {
            playerMovement.canMove = false;
        }
        
        // Remove gun if configured
        if (spawnConfig != null && spawnConfig.removeGunsOnScoreboard)
        {
            playerMovement.RemoveGun();
        }
        
        // Play ending animation
        if (playerMovement.animator != null && spawnConfig != null)
        {
            string animationName = playerIndex == 0 ? 
                spawnConfig.winnerAnimationName : 
                spawnConfig.loserAnimationName;
                
            playerMovement.animator.SetBool(animationName, true);
        }
    }
}
