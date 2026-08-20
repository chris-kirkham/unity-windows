using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

class AutoLoadBootstrapScene
{
    private const int BoostrapSceneIndex = (int)SceneBuildIndex.Bootstrap;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void LoadBoostrapScene()
    {
        var activeSceneIndexes = new List<int>();

        for(int i = 0; i < SceneManager.sceneCount; i++) //TODO: should be .loadedSceneCount?
        {
            activeSceneIndexes.Add(SceneManager.GetSceneAt(i).buildIndex);
        }

        var isBootstrapSceneOpen = activeSceneIndexes.Contains(BoostrapSceneIndex);
        if (!isBootstrapSceneOpen) 
        {
            Debug.Log("Unloading active scenes...");
            foreach(var buildIdx in activeSceneIndexes)
            {
                SceneManager.UnloadScene(buildIdx); //TODO: I don't want this to be async!
            }
            
            Debug.Log("Loading boostrap scene...");
            
            SceneManager.LoadScene(BoostrapSceneIndex, LoadSceneMode.Single);
         
            Debug.Log("Bootstrap scene loaded! Reloading other scene(s)...");

            foreach (var buildIdx in activeSceneIndexes)
            {
                SceneManager.LoadScene(buildIdx, LoadSceneMode.Additive);
            }

            Debug.Log("All scene(s) loaded!");
        }
    }
}
