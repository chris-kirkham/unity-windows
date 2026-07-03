using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHUD : MonoBehaviour
{
    [SerializeField] private Image currHealthImg;
    [SerializeField] private TextMeshProUGUI currHealthText;

    private int playerMaxHealth;


    public void SetCurrentHealth(int health)
    {
        if(currHealthImg)
        {
            if(playerMaxHealth > 0)
            {
                currHealthImg.fillAmount = health / playerMaxHealth;
            }
        }

        if(currHealthText)
        {
            currHealthText.text = $"{health}/{playerMaxHealth}";
        }
    }
}
