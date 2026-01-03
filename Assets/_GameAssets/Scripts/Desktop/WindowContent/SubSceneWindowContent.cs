using UnityEngine;
using UnityEngine.SceneManagement;

public class SubSceneWindowContent : WindowContent
{
    private Scene scene;

    public void SetModelData(ModelViewerModelData data)
    {
    
    }

    [ContextMenu("DEBUG: Load content")]
    public override void LoadContent(Window window)
    {
        //TODO: display model to RenderTexture in window - create new scene per model w/ model prefab, background etc?
        scene = SceneManager.CreateScene(gameObject.name, new CreateSceneParameters(LocalPhysicsMode.Physics3D));
    }

    [ContextMenu("DEBUG: Unload content")]
    public override void UnloadContent(Window window)
    {
        SceneManager.UnloadSceneAsync(scene.name);
    }
}
