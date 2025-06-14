using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    public static GameManager Instance;

    public enum GameState { Start, Lobby, Menu, Playing, ScoreBoard, Paused, GameOver }
    public GameState currentState { get; private set; }
    public LobbyManager lobbyManager;
    
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


    public void GameOver()
    {
        Debug.Log("Game Over");
        UpdateState(GameState.GameOver);
    }

    public void RestartGame()
    {
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