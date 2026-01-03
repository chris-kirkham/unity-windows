using UnityEngine;

public class WindowContent : MonoBehaviour
{
    public Vector2 desiredWindowSize;

    public virtual void LoadContent(Window window) {}

    public virtual void UnloadContent(Window window) {}
}
