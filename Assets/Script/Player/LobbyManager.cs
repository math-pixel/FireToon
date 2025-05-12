using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class LobbyManager : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            CustomSceneManager.Instance.ChangeScene("InGame");
        }
    }

    private void OnPlayerJoined(PlayerInput playerInput)
    {
        PlayerRegistry.Instance.RegisterPlayer(playerInput);
    }

    // Appelé pour passer à la scène suivante
    public void LoadGameScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("GameScene");
    }
}