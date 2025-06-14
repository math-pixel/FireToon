using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class LobbyManager : MonoBehaviour
{
    private PlayerRegistry _playerRegistry;
    public Timer Timer;
    private bool timerStarted = false;

    private void Start()
    {
        GameManager.Instance.lobbyManager = this;
        _playerRegistry = gameObject.GetComponent<PlayerRegistry>();
    }

    private void Update()
    {

        if (_playerRegistry.IsAllPlayersRegistered())
        {
            if (!timerStarted)
            {
                timerStarted = true;
                Timer.StartCountdown(3, changeScene);
            }
        }
        
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
        Debug.Log("New Player joined");
        PlayerRegistry.Instance.RegisterPlayer(playerInput, playerInput.playerIndex);
    }

}