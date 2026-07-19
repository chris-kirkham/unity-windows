using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHUD_PlayerInfo : MonoBehaviour
{
    [SerializeField] private Image currHealthImg;
    [SerializeField] private TextMeshProUGUI currHealthText;

    private Player player;

    public void Initialise(Player player)
    {
        this.player = player;
        player.OnHealthChange += UpdateHealth;
    }

    private void OnDisable()
    {
        if(player)
        {
            player.OnHealthChange -= UpdateHealth;
        }
    }

    public void UpdateHealth(int health)
    {
        if(!player)
        {
            return;
        }

        if(currHealthImg)
        {
            if(player.MaxHealth > 0)
            {
                currHealthImg.fillAmount = player.CurrHealth / player.MaxHealth;
            }
        }

        if(currHealthText)
        {
            currHealthText.text = $"{player.CurrHealth}/{player.MaxHealth}";
        }
    }
}
