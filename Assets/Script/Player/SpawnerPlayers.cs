using System;
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
        Debug.Log(players);
        for (int i = 0; i < players.Count && i < spawnPoints.Length; i++)
        {
            var player = players[i];
            player.gameObject.SetActive(true);
            player.transform.position = spawnPoints[i].position;
            player.transform.rotation = spawnPoints[i].rotation;

            // S'assurer que les composants nécessaires sont réinitialisés si besoin
            // player.GetComponent<PlayerController>()?.ResetPlayer();

            if (GameManager.Instance.currentState == GameManager.GameState.ScoreBoard)
            {
                // refuse mouvement if its on ending scene
                PlayerMovement playerMovement = player.GetComponent<PlayerMovement>();
                playerMovement.canMove = false;
                
                // remove gun from final podium
                playerMovement.removeGun();
                
                // play ending animation
                String AnimationName = i == 0 ? "EndingAnimationWinner" : "EndingAnimationLooser";
                playerMovement.animator.SetBool(AnimationName, true);
            }
        }
    }
}
