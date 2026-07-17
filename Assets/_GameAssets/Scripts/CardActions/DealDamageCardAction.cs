using DG.Tweening;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

[CreateAssetMenu(fileName = "CardAction_DealDamage", menuName = "Card Actions/Deal Damage")]
public class DealDamageCardAction : CardAction
{
    [SerializeField] private DealDamageActionVFX vfxPrefab;
    [SerializeField] private ITargetable.TargetableType damageTarget;
    [SerializeField] private int damage;
    [SerializeField] private float timeToReachTarget = 1f;

    public override async Task Execute()
    {
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
            await vfx.DoVFX(item.transform.position, Target.GetPositionAsTarget(), timeToReachTarget, Target.GetTargetType());
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
