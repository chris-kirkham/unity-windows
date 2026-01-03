using System.Collections.Generic;
using UnityEngine;

public class WindowManager : SingletonMonoBehaviour<WindowManager>
{
    //TODO: Should windows be able to be created/destroyed outside WindowManager? Or should everything go through WindowManager

    [SerializeField] private Window windowPrefab;
    [SerializeField] private Taskbar taskbar;

    private List<Window> activeWindows;

    private void OnEnable()
    {
        if(!windowPrefab)
        {
            Debug.LogError($"{nameof(WindowManager)} has no {nameof(Window)} prefab set!");
        }

        activeWindows 
            = new List<Window>(FindObjectsByType<Window>(FindObjectsInactive.Exclude, FindObjectsSortMode.None));
    }

    public void CreateWindow()
    {
        activeWindows.Add(InstantiateNewWindowFromPrefab());
    }

    public void CreateWindow(WindowContent content)
    {
        var window = InstantiateNewWindowFromPrefab();
        window.SetWindowContent(content);
        activeWindows.Add(window);
    }

    private Window InstantiateNewWindowFromPrefab()
    {
        return Instantiate<Window>(windowPrefab, Vector3.zero, Quaternion.identity);
    }
}
