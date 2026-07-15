using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "CardAction_DealDamage", menuName = "Card Actions/Deal Damage")]
public class DealDamageCardAction : CardAction
{
    public enum DamageTarget
    {
        Card,
        Player
    }

    [SerializeField] private DamageTarget damageTarget;
    [SerializeField] private int damage;

    private Player targetPlayer;

    public override async Task Execute()
    {
        await DoDamageFX();
        DealDamage();
    }

    private async Task DoDamageFX()
    {
        await Task.Delay(1000);
    }

    private void DealDamage()
    {
        if(!targetPlayer)
        {
            Debug.LogError("Target player is null!");
            return;
        }

        targetPlayer.DealDamage(damage);
    }
}
