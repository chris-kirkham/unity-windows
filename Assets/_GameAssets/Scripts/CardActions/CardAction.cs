using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public abstract class CardAction : ScriptableObject, IGameAction
{
    public enum TargetingBehaviour
    {
        NoTarget,
        PlayerChoosesTargets,
        RandomTargets
    }

    [field: SerializeField] public string ActionName { get; private set; }
    [field: SerializeField] public string ActionDesc { get; private set; }

    [field: SerializeField] public TargetingBehaviour targetingBehaviour { get; private set; }

    [field: SerializeField, Min(0)] public int NumTargets { get; private set; }

    protected Player owningPlayer;
    protected CraftingItem item;

    public List<ITargetable> Targets { protected get; set; }

    public void Initialise(Player owningPlayer, CraftingItem item)
    {
        this.owningPlayer = owningPlayer;
        this.item = item;
    }

    protected async Task DoTargeting()
    {
        Targets = await owningPlayer.DoPlayerTargeting(targetingBehaviour, NumTargets);
    }

    public async Task Execute()
    {
        await Execute_PreTargeting();
        await DoTargeting();
        await Execute_PostTargeting();
    }

    protected async virtual Task Execute_PreTargeting()
    {
    }

    protected async virtual Task Execute_PostTargeting()
    {
    }

    public abstract void Cancel();
}
