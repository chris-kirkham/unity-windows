using UnityEngine;

public abstract class SingletonMonoBehaviour<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _inst;

    public static T Inst
    {
        get
        {
            if (_inst)
            {
                return _inst;
            }

            Debug.LogErrorFormat($"No instance of {0} found!", nameof(T));
            return null;
        }
    }

    private void Awake()
    {
        if (_inst)
        {
            Debug.LogWarningFormat($"Instance of {0} already exists! Destroying this instance.", nameof(T));
            Destroy(gameObject);
        }
        else
        {
            _inst = this as T;
        }

        OnAwake();
    }

    private void OnDestroy()
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
