using Fusion;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//https://doc.photonengine.com/fusion/v2/tutorials/shared-mode-basics/2-scene-and-player
public class PlayerSpawner : SimulationBehaviour, IPlayerJoined
{
    [SerializeField] private GameObject playerBoardSidePrefab;
    [SerializeField] private List<Transform> playerSpawnPoints;

    void IPlayerJoined.PlayerJoined(PlayerRef playerRef)
    {
        if(playerRef == Runner.LocalPlayer)
        {
            var playerIdx = playerRef.AsIndex - 1;

            if(playerIdx >= playerSpawnPoints.Count)
            {
                Debug.LogError($"More players than spawn points! This shouldn't happen");
                return;
            }

            //spawn player on game board
            var playerBoardSide = Runner.Spawn(playerBoardSidePrefab);
            playerBoardSide.transform.parent = playerSpawnPoints[playerIdx];
            playerBoardSide.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            Runner.SetPlayerObject(playerRef, playerBoardSide);
            if(GameManager.InstExists())
            { 
               GameManager.Inst.RegisterPlayer(playerBoardSide.GetComponentInChildren<Player>(), playerRef); //TODO: REFACTOR WHERE PLAYER CLASS IS AND ITS RELATION TO PLAYER BOARD ETC!
            }
        }
    }
}
