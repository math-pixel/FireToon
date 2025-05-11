using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerRegistry : MonoBehaviour
{
    public static PlayerRegistry Instance;
    public List<PlayerInput> RegisteredPlayers = new List<PlayerInput>();

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void RegisterPlayer(PlayerInput input)
    {
        RegisteredPlayers.Add(input);
        DontDestroyOnLoad(input.gameObject); // garde le joueur à travers les scènes
    }

    public void Clear()
    {
        foreach (var player in RegisteredPlayers)
            Destroy(player.gameObject);

        RegisteredPlayers.Clear();
    }
}
