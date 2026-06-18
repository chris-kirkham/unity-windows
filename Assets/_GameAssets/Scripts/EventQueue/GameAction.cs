using System;
using System.Threading.Tasks;

public abstract class GameAction
{
    public GameAction()
    {
    }

    public abstract Task Execute();
}
