using System.Collections.Generic;

public class GameManager : SingletonMonoBehaviour<GameManager>
{
    private List<Player> players;

    public Player GetPlayer(int playerIndex)
    {
        if(playerIndex < 0 || playerIndex >= players.Count)
        {
            throw new System.IndexOutOfRangeException();
        }

        return players[playerIndex];
    }
}
