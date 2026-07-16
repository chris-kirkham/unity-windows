using Fusion;
using System.Collections.Generic;

public class GameManager : SingletonNetworkBehaviour<GameManager>, IPlayerJoined, IPlayerLeft
{
    private List<PlayerRef> playerRefs;
    private List<Player> players;

    public Player GetPlayer(int playerIndex)
    {
        if(playerIndex < 0 || playerIndex >= players.Count)
        {
            throw new System.IndexOutOfRangeException();
        }

        return players[playerIndex];
    }

    void IPlayerJoined.PlayerJoined(PlayerRef player)
    {
        playerRefs.Add(player);
    }

    void IPlayerLeft.PlayerLeft(PlayerRef player)
    {
        playerRefs.Remove(player);
    }
}
