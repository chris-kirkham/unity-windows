using System.Threading;
using System.Threading.Tasks;

public interface IGameAction
{
    public Task Execute();

    public void Cancel(); //TODO: task cancellation token?
}