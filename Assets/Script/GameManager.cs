using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    public static GameManager Instance;

    public enum GameState { Start, Lobby, Menu, Playing, ScoreBoard, Paused, GameOver }
    public GameState currentState { get; private set; }
    public LobbyManager lobbyManager;
    
    public List<PlayerInput> LeaderboardPlayers = new List<PlayerInput>();
    
    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        UpdateState(GameState.Start);
        DontDestroyOnLoad(gameObject);
    }

    public void PlayerDead(PlayerInput player)
    {
        LeaderboardPlayers.Insert(0, player);

        // if there is only one survival
        if (LeaderboardPlayers.Count == PlayerRegistry.Instance.RegisteredPlayers.Count - 1)
        {
            // hhow to add the last survival 
            // Trouve le dernier joueur vivant
            PlayerInput lastSurvivor = PlayerRegistry.Instance.RegisteredPlayers
                .First(p => !LeaderboardPlayers.Contains(p));

            // L'ajoute en dernier au leaderboard
            LeaderboardPlayers.Insert(0, lastSurvivor);
            
            GameOver();
        }
    }

    public void GameOver()
    {
        Debug.Log("Game Over");
        UpdateState(GameState.GameOver);
    }

    public void RestartGame()
    {
        LeaderboardPlayers = new List<PlayerInput>();
        PlayerRegistry.Instance.Clear();
        Destroy(lobbyManager?.gameObject);
        UpdateState(GameState.Start);
    }

    // ReSharper disable Unity.PerformanceAnalysis
    public void UpdateState(GameState newState)
    {
        currentState = newState;

        switch (currentState)
        {
            case GameState.Start:
                ChangeScene("MainMenu");
                break;
            case GameState.Lobby:
                ChangeScene("LobbyScene");
                break;
            case GameState.Playing:
                ChangeScene("InGame");
                break;
            case GameState.GameOver:
                UpdateState(GameState.ScoreBoard);
                break;
            case GameState.ScoreBoard:
                ChangeScene("EndingScene");
                break;
            default:
                // display an error
                Debug.Log("Bad Game State");
                break;
        }
    }
    
    public void ChangeScene(String sceneName)
    {
        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
    }
}