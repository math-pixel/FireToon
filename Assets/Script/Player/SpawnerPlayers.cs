using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerPlayers : MonoBehaviour
{
    public Transform[] spawnPoints;

    void Start()
    {
        var players = PlayerRegistry.Instance.RegisteredPlayers;

        for (int i = 0; i < players.Count && i < spawnPoints.Length; i++)
        {
            var player = players[i];
            player.transform.position = spawnPoints[i].position;
            player.transform.rotation = spawnPoints[i].rotation;

            // S'assurer que les composants nécessaires sont réinitialisés si besoin
            // player.GetComponent<PlayerController>()?.ResetPlayer();
        }
    }
}
