using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public abstract class CardAction : ScriptableObject, IGameAction
{
    [field: SerializeField] public string ActionName { get; private set; }
    [field: SerializeField] public string ActionDesc { get; private set; }
    [field: SerializeField, Min(0)] public int NumTargets { get; private set; }

    protected Player owningPlayer;
    protected CraftingItem item;

    
    public void Initialise(Player owningPlayer, CraftingItem item)
    {
        this.owningPlayer = owningPlayer;
        this.item = item;
    }

    public abstract Task Execute();

    public abstract void Cancel();
}
