using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class TargetedCardAction : CardAction
{
    public enum TargetingBehaviour
    {
        PlayerChoosesTargets,
        RandomTargets
    }

    public enum TargetTeam
    {
        Friendly,
        Enemy
    }

    [field: SerializeField] public TargetingBehaviour targetingBehaviour { get; private set; }
    [field: SerializeField] public TargetTeam targetTeam { get; private set; } 
    [field: SerializeField] public ITargetable.TargetableType targetableTypeMask { get; private set; }

    public List<ITargetable> Targets { protected get; set; }

    protected async Task DoTargeting(Transform thisItemTform, TargetingBehaviour targetingBehaviour, TargetTeam targetTeam, int numTargets)
    {
        Targets = await owningPlayer.DoPlayerTargeting(thisItemTform, targetableTypeMask, targetingBehaviour, targetTeam, numTargets);
    }

    public async override Task Execute()
    {
        await Execute_PreTargeting();
        await DoTargeting(item.transform, targetingBehaviour, targetTeam, NumTargets);
        await Execute_PostTargeting();
    }

    protected async virtual Task Execute_PreTargeting()
    {
    }

    protected async virtual Task Execute_PostTargeting()
    {
    }

    public override void Cancel()
    {
        throw new System.NotImplementedException();
    }
}
