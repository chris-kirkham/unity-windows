using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "CardAction_Test", menuName = "Card Actions/Test")]
public class TestCardAction : CardAction, IGameAction
{
    protected override async Task Execute_PostTargeting()
    {
        await new Task(() => { Debug.Log("Test action executed!"); });
    }

    public override void Cancel()
    {
        throw new System.NotImplementedException();
    }
}
