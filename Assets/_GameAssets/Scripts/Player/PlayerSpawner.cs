using Fusion;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//https://doc.photonengine.com/fusion/v2/tutorials/shared-mode-basics/2-scene-and-player
public class PlayerSpawner : SimulationBehaviour, IPlayerJoined
{
    [SerializeField] private GameObject playerBoardSidePrefab;
    [SerializeField] private List<Transform> playerSpawnPoints;

    void IPlayerJoined.PlayerJoined(PlayerRef player)
    {
        if(player == Runner.LocalPlayer)
        {
            var playerIdx = player.AsIndex;

            if(playerIdx >= playerSpawnPoints.Count)
            {
                Debug.LogError($"More players than spawn points! This shouldn't happen");
                return;
            }

            //spawn player on game board
            var playerBoardSide = Runner.Spawn(playerBoardSidePrefab);
            playerBoardSide.transform.parent = playerSpawnPoints[playerIdx];
            playerBoardSide.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            Runner.SetPlayerObject(player, playerBoardSide);
        }
    }
}
