using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class InGameManager : MonoBehaviour
{
    
    [SerializeField]
    CinemachineTargetGroup cinemachineTargetGroup;
    
    // Start is called before the first frame update
    void Start()
    {
        initializeCamera();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void initializeCamera()
    {
        var players = PlayerRegistry.Instance.RegisteredPlayers;

        for (int i = 0; i < players.Count; i++)
        {
            cinemachineTargetGroup.AddMember(players[i].transform, 1f, 5f);
        }
    }
}
