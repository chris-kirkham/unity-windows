using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

//https://docs.unity3d.com/6000.2/Documentation/Manual/async-awaitable-continuations.html
public class GameActionQueue<T> where T : IGameAction
{
    private Queue<T> queue;
    private T currentAction;
    bool actionLoopRunning;

    public bool doDebugLog;

    public GameActionQueue()
    {
        queue = new Queue<T>();
    }

    public void EnqeueAction(T action)
    {
        queue.Enqueue(action);

        if (doDebugLog)
        {
            Debug.Log($"Enqueued action {nameof(T)} on queue {this.ToString()} - queue size {queue.Count}");
        }

        if(!actionLoopRunning)
        {
            if (doDebugLog)
            {
                Debug.Log($"Starting action loop...");
            }

            DoActionLoop();
        }
    }

    private async void DoActionLoop()
    {
        actionLoopRunning = true;

        while (queue.Count > 0)
        {
            if(doDebugLog)
            {
                Debug.Log($"Executing action {queue.Peek().GetType().ToString()}");
            }

            currentAction = queue.Dequeue();
            await currentAction.Execute();
        }

        actionLoopRunning = false;
    }
}

