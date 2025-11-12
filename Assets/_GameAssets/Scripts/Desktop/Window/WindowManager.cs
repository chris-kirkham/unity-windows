using System.Collections.Generic;
using UnityEngine;

public class WindowManager : SingletonMonoBehaviour<WindowManager>
{
    [SerializeField] private Taskbar taskbar;

    private Window[] windows;

    private void OnEnable()
    {
        windows = FindObjectsByType<Window>(FindObjectsInactive.Include, FindObjectsSortMode.None);
    }

}
