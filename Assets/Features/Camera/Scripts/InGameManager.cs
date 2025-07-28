using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class InGameManager : MonoBehaviour
{
    [Header("Configuration")]
    public CameraConfig cameraConfig;
    
    [Header("References")]
    [SerializeField] private CinemachineTargetGroup cinemachineTargetGroup;
    
    private List<PlayerInput> trackedPlayers = new List<PlayerInput>();

    void Start()
    {
        InitializeCamera();
        
        if (GameManager.Instance != null)
        {
            // Subscribe to player death events if needed
        }
    }

    public void InitializeCamera()
    {
        if (PlayerRegistry.Instance == null || cameraConfig == null) return;
        
        var players = PlayerRegistry.Instance.RegisteredPlayers;
        
        if (!cameraConfig.autoAddPlayers) return;

        foreach (var player in players)
        {
            AddPlayerToCamera(player);
        }
    }
    
    public void AddPlayerToCamera(PlayerInput player)
    {
        if (cinemachineTargetGroup == null || cameraConfig == null) return;
        
        if (!trackedPlayers.Contains(player))
        {
            cinemachineTargetGroup.AddMember(
                player.transform, 
                cameraConfig.defaultWeight, 
                cameraConfig.defaultRadius
            );
            trackedPlayers.Add(player);
        }
    }
    
    public void RemovePlayerFromCamera(PlayerInput player)
    {
        if (cinemachineTargetGroup == null || !cameraConfig.removeDeadPlayers) return;
        
        if (trackedPlayers.Contains(player))
        {
            cinemachineTargetGroup.RemoveMember(player.transform);
            trackedPlayers.Remove(player);
        }
    }
    
    public void UpdateCameraTarget(PlayerInput player, float weight, float radius)
    {
        if (cinemachineTargetGroup == null) return;
        
        var targetGroup = cinemachineTargetGroup;
        for (int i = 0; i < targetGroup.m_Targets.Length; i++)
        {
            if (targetGroup.m_Targets[i].Object == player.transform)
            {
                targetGroup.m_Targets[i].Weight = weight;
                targetGroup.m_Targets[i].Radius = radius;
                break;
            }
        }
    }
}
