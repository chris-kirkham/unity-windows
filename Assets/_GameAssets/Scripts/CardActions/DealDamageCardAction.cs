using DG.Tweening;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "CardAction_DealDamage", menuName = "Card Actions/Deal Damage")]
public class DealDamageCardAction : CardAction
{
    [SerializeField] private DealDamageActionVFX vfxPrefab;
    [SerializeField] private ITargetable.TargetableType damageTarget;
    [SerializeField] private int damage;
    [SerializeField] private float timeToReachTarget = 1f;

    protected override async Task Execute_PostTargeting()
    {
        if(Targets == null)
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
            var targetTasks = new Task[Targets.Count];
            for(int i = 0; i < Targets.Count; i++)
            {
                var target = Targets[i];
                var vfx = Instantiate<DealDamageActionVFX>(vfxPrefab, item.transform.position, item.transform.rotation);
                targetTasks[i] = vfx.DoVFX(item.transform.position, target.GetTargetPosition(), timeToReachTarget, target.GetTargetType());
            }

            await Task.WhenAll(targetTasks);
        }
        else
        {
            await Task.Delay(1000);
        }
    }

    private void DealDamage()
    {
        Debug.Log($"TODO: deal damage to card/player!");
    }
}
