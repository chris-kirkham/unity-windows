using System.Threading.Tasks;
using UnityEngine;

public class DealDamageCardAction : CardAction
{
    public enum DamageTarget
    {
        Card,
        Player
    }

    [SerializeField] private DamageTarget damageTarget;
    [SerializeField] private int damage;

    public override Task Execute()
    {
        throw new System.NotImplementedException();       
    }
}
