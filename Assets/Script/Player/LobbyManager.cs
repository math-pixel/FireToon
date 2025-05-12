using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class LobbyManager : MonoBehaviour
{
    private void Update()
    {
        // Event temporary
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            changeScene();
        }
    }

    private void changeScene()
    {
        GameManager.Instance.UpdateState(GameManager.GameState.Playing);
    }

    private void OnPlayerJoined(PlayerInput playerInput)
    {
        
        PlayerRegistry.Instance.RegisterPlayer(playerInput, playerInput.playerIndex);
    }

}