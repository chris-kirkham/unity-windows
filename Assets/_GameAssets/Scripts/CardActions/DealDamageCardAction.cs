using DG.Tweening;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

[CreateAssetMenu(fileName = "CardAction_DealDamage", menuName = "Card Actions/Deal Damage")]
public class DealDamageCardAction : CardAction
{
    public enum DamageTarget
    {
        Card,
        Player
    }

    [SerializeField] private DealDamageActionVFX vfxPrefab;
    [SerializeField] private DamageTarget damageTarget;
    [SerializeField] private int damage;
    [SerializeField] private float timeToReachTarget = 1f;

    private Player targetPlayer;

    public override async Task Execute()
    {
        //TEST
        Target = owningPlayer;
        targetPlayer = owningPlayer;

        if(Target == null)
        {
            Debug.LogError($"Target is required for this action!");
            return;
        }

        await DoDamageFX();
        DealDamage();
    }

    public override void Cancel()
    {
        throw new System.NotImplementedException();
    }
        
    private async Task DoDamageFX()
    {
        if(vfxPrefab)
        {
            var vfx = Instantiate<DealDamageActionVFX>(vfxPrefab, item.transform.position, item.transform.rotation);
            await vfx.DoVFX(item.transform.position, targetPlayer.GetTargetPosition(), timeToReachTarget);
        }
        else
        {
            await Task.Delay(1000);
        }
    }

    private void DealDamage()
    {
        if(!targetPlayer)
        {
            Debug.LogError("Target player is null!");
            return;
        }

        targetPlayer.Damage(damage);
    }
}
