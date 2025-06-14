using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class SpawnerPlayers : MonoBehaviour
{
    public Transform[] spawnPoints;

    private List<PlayerInput> _playerInputs;
    
    void Start()
    {

        // display player
        if (GameManager.Instance.currentState == GameManager.GameState.Playing)
        {
            _playerInputs = PlayerRegistry.Instance.RegisteredPlayers;
        }else if (GameManager.Instance.currentState == GameManager.GameState.ScoreBoard)
        {
            _playerInputs = GameManager.Instance.LeaderboardPlayers;
        }
        SpawnPlayers(_playerInputs);
    }

    void SpawnPlayers(List<PlayerInput> players)
    {
        for (int i = 0; i < players.Count && i < spawnPoints.Length; i++)
        {
            var player = players[i];
            player.gameObject.SetActive(true);
            player.transform.position = spawnPoints[i].position;
            player.transform.rotation = spawnPoints[i].rotation;

            // S'assurer que les composants nécessaires sont réinitialisés si besoin
            // player.GetComponent<PlayerController>()?.ResetPlayer();
        }
    }
}
