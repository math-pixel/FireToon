using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerRegistry : MonoBehaviour
{
    public static PlayerRegistry Instance;
    public List<PlayerInput> RegisteredPlayers = new List<PlayerInput>();
    
    public List<GameObject> SkinList = new List<GameObject>();

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

    public void RegisterPlayer(PlayerInput input, int playerNumber)
    {
        RegisteredPlayers.Add(input);
        // change Name player
        input.gameObject.name = $"Player {playerNumber}";
        
        // Change skin of player
        GameObject skin = Instantiate(SkinList[0], input.gameObject.transform);
        skin.transform.localScale = new Vector3(1, 1, 1) * 2.88f;
        skin.transform.localPosition = new Vector3(0, -0.9f, 0);
        
        // save player across scene
        DontDestroyOnLoad(input.gameObject); // garde le joueur à travers les scènes
        
    }

    public void Clear()
    {
        foreach (var player in RegisteredPlayers)
            Destroy(player.gameObject);

        RegisteredPlayers.Clear();
    }
}
