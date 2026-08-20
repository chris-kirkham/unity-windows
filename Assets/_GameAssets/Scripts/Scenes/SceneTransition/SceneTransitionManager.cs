using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitionManager : SingletonMonoBehaviour<SceneTransitionManager>
{
    [SerializeField] private int initialSceneBuildIdx;
    [SerializeField] private int loadingSceneBuildIdx;

    [ContextMenu("Load initial scene")]
    private void LoadInitialScene()
    {
        SceneManager.LoadScene(initialSceneBuildIdx, LoadSceneMode.Single);
    }

    //TODO: implement callback here so we can unload stuff manually on scene change?
    public async void LoadScene(int sceneBuildIdx, bool useLoadingScreen = true)
    {
        if(sceneBuildIdx < 0 || sceneBuildIdx >= SceneManager.sceneCountInBuildSettings)
        {
            Debug.LogErrorFormat($"Given scene build index {0} is invalid! Cannot load scene.", sceneBuildIdx);
            return;
        }

        //get scene about to be unloaded
        var prevScene = SceneManager.GetActiveScene();

        //load loading scene additively before unloading active scene
        await SceneManager.LoadSceneAsync(loadingSceneBuildIdx, LoadSceneMode.Additive);

        if (LoadingScreen.InstExists())
        {
            await LoadingScreen.Inst.FadeInRoutine();
        }
        else
        {
            Debug.LogError($"Instance of {nameof(LoadingScreen)} not present! It should have been loaded by now!");
        }

        SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(loadingSceneBuildIdx));

        //unload previous scene, if any
        if (prevScene.IsValid())
        {
            await SceneManager.UnloadSceneAsync(prevScene);
        }

        //load target scene async
        var async = SceneManager.LoadSceneAsync(sceneBuildIdx, LoadSceneMode.Additive); 
        while(!async.isDone)
        {
            if(LoadingScreen.InstExists())
            {
                LoadingScreen.Inst.SetProgressBar(async.progress);
            }

            await Task.Yield();
        }

        //unload loading scene async
        async = SceneManager.UnloadSceneAsync(loadingSceneBuildIdx);
        if(LoadingScreen.InstExists())
        {
            await LoadingScreen.Inst.FadeOutRoutine();
        }

        await async;
    }
}
