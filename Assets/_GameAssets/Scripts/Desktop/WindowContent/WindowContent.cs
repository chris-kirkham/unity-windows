using UnityEngine;

public abstract class WindowContent : MonoBehaviour
{
    public abstract Texture GetContentTexture();

    public abstract void LoadContent();

    public abstract void UnloadContent();
}
