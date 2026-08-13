using DG.Tweening;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "CardAction_DealDamage", menuName = "Card Actions/Deal Damage")]
public class ProjectileCardAction : TargetedCardAction
{
    [SerializeField] private ProjectileVFX vfxPrefab;
    [SerializeField, FormerlySerializedAs("damageTarget")] private ITargetable.TargetableType targetType;
    [SerializeField] private int damage;
    [SerializeField] private float timeToReachTarget = 1f;

    protected override async Task Execute_PostTargeting()
    {
        if(Targets == null)
        {
            Debug.LogError($"Target is required for this action!");
            return;
        }

        await DoProjectileVFX();
        DealDamage();
    }

    public override void Cancel()
    {
        throw new System.NotImplementedException();
    }
        
    private async Task DoProjectileVFX()
    {
        if(vfxPrefab)
        {
            var targetTasks = new Task[Targets.Count];
            for(int i = 0; i < Targets.Count; i++)
            {
                var target = Targets[i];
                var vfx = Instantiate<ProjectileVFX>(vfxPrefab, item.transform.position, item.transform.rotation);
                targetTasks[i] = vfx.DoVFX(item.transform.position, target.GetTargetPosition(), timeToReachTarget, target.GetTargetableType());
            }

            await Task.WhenAll(targetTasks);
        }
        else
        {
            await Task.Delay((int)timeToReachTarget * 1000);
        }
    }

    private void DealDamage()
    {
        foreach(var target in Targets)
        {
            if(target is IHaveHealth)
            {
                ((IHaveHealth)target).DamageHealth(damage);
            }
            else
            {
                //TODO: REFACTOR TARGETING SO WE DON'T NEED TO CHECK THIS!
                Debug.LogError($"Targeted something which isn't damageable! This should have been caught in the targeting step!");
            }
        }
    }
}
