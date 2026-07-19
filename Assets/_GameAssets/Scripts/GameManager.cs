using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class GameManager : SingletonNetworkBehaviour<GameManager>
{
    private List<PlayerRef> playerRefs = new List<PlayerRef>();
    private List<Player> players = new List<Player>();

    public Player GetPlayer(int playerIndex)
    {
        if(playerIndex < 0 || playerIndex >= players.Count)
        {
            throw new System.IndexOutOfRangeException($"Player index {playerIndex} out of range of player list!");
        }

        return players[playerIndex];
    }

    public Player GetPlayer(PlayerRef playerRef)
    {
        //TODO: refactor player/playerRef integration
        var playerIdx = playerRefs.IndexOf(playerRef);
        if(playerIdx < 0)
        {
            throw new System.IndexOutOfRangeException($"{nameof(PlayerRef)} {playerRef.AsIndex} not found in player list!");
        }

        return players[playerIdx];
    }

    public void RegisterPlayer(Player player, PlayerRef playerRef)
    {
        players.Add(player);
        playerRefs.Add(playerRef);
    }

    public void UnregisterPlayer(Player player, PlayerRef playerRef)
    {
        throw new System.NotImplementedException();
    }
}
