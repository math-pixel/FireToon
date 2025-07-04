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
        _playerRegistry = gameObject.GetComponent<PlayerRegistry>();
     
        // add lobby script to game manger for detroy at the end of the party
        GameManager.Instance.lobbyManager = this;
        
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
        if (GameManager.Instance?.currentState == GameManager.GameState.Lobby)
        {
            PlayerRegistry.Instance.RegisterPlayer(playerInput, playerInput.playerIndex);
        }
    }

}