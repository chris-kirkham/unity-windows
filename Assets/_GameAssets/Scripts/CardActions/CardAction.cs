using System.Threading.Tasks;
using UnityEngine;

public abstract class CardAction : ScriptableObject, IGameAction
{
    [field: SerializeField] public string ActionName { get; private set; }
    [field: SerializeField] public string ActionDesc { get; private set; }

    [field: SerializeField] public bool IsTargeted { get; private set; } //does this action need to target something?

    protected Player owningPlayer;
    protected CraftingItem item;

    public ITargetable Target { protected get; set; }

    public void Initialise(Player owningPlayer, CraftingItem item)
    {
        this.owningPlayer = owningPlayer;
        this.item = item;
    }

    public abstract Task Execute();

    public abstract void Cancel();
}
