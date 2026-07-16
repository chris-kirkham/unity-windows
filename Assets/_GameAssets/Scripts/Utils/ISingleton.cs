using UnityEngine;
using Fusion;

public interface ISingleton<T> where T : MonoBehaviour
{
    protected static T _inst;

    public static T Inst
    {
        get
        {
            if (_inst )
            {
                return _inst;
            }

            return null;
        }
    }

    public void Awake(GameObject thisGameObject)
    {
        if (_inst)
        {
            Debug.LogWarning($"Instance of {typeof(T).Name} already exists! Destroying this instance.");
            GameObject.Destroy(thisGameObject);
        }
        else
        {
            _inst = this as T;
        }

        OnAwake();
    }

    public void OnDestroy()
    {
        if (_inst == this)
        {
            _inst = null;
        }
    }

    public static bool InstExists()
    {
        return _inst;
    }

    protected virtual void OnAwake()
    {

    }
}
