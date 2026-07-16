using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "CardAction_Test", menuName = "Card Actions/Test")]
public class TestCardAction : CardAction, IGameAction
{
    public override Task Execute()
    {
        return new Task(() => { Debug.Log("Test action executed!"); });
    }

    public override void Cancel()
    {
        throw new System.NotImplementedException();
    }
}
