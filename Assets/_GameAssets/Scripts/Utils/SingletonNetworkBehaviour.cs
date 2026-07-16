using UnityEngine;
using Fusion;

public abstract class SingletonNetworkBehaviour<T> : NetworkBehaviour, ISingleton<T> where T : NetworkBehaviour
{
    public static T Inst => ISingleton<T>.Inst;

    public static bool InstExists()
    {
        return ISingleton<T>.InstExists();
    }

    protected virtual void Awake()
    {
        ((ISingleton<T>)this).Awake(gameObject);
    }

    protected virtual void OnDestroy()
    {
        ((ISingleton<T>)this).OnDestroy();
    }
}
