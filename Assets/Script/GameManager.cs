using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    public static GameManager Instance;

    public enum GameState { Start, Lobby, Menu, Playing, ScoreBoard, Paused, GameOver }
    public GameState CurrentState { get; private set; }

    
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

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GameOver()
    {
        Debug.Log("Game Over");
        UpdateState(GameState.GameOver);
    }

    // ReSharper disable Unity.PerformanceAnalysis
    public void UpdateState(GameState newState)
    {
        CurrentState = newState;

        switch (CurrentState)
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