using System.Collections.Generic;
using UnityEngine;
using Fusion;
using System.Linq;

[System.Serializable]
public class PlayerHUD
{
    [SerializeField] private List<PlayerHUD_PlayerInfo> playerHUDInfos;

    private NetworkRunner runner;

    public void Initialise(NetworkRunner runner)
    {
        this.runner = runner;

        var players = runner.ActivePlayers.ToList();
        var playerCount = players.Count;
        
        if(playerCount > playerHUDInfos.Count)
        {
            Debug.LogError($"More players than {nameof(PlayerHUD_PlayerInfo)}s set!");
        }

        for(int i = 0; i < playerHUDInfos.Count; i++)
        {
            if(i >= playerCount)
            {
                playerHUDInfos[i].gameObject.SetActive(false);
            }
            else
            {
                playerHUDInfos[i].Initialise(GameManager.Inst.GetPlayer(i));
            }
        }
    }

    public void UpdateHUD()
    {
        
    }
}
