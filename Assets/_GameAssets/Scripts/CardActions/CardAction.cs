using System.Threading.Tasks;
using UnityEngine;

public abstract class CardAction : ScriptableObject, IGameAction
{
    [field: SerializeField] public string ActionName { get; private set; }

    public abstract Task Execute();
}
