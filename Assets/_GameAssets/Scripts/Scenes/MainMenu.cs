using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private Button playButton;
    [SerializeField] private Button galleryButton;
    [SerializeField] private int mainSceneIdx;
    [SerializeField] private int gallerySceneIdx;

    private SceneTransitionManager sceneTransitionManager;

    private void Start()
    {
        sceneTransitionManager = SceneTransitionManager.Inst;
    
        if(sceneTransitionManager)
        {
            playButton.onClick.AddListener(() => sceneTransitionManager.LoadSceneWithLoadingScreen(mainSceneIdx));
            galleryButton.onClick.AddListener(() => sceneTransitionManager.LoadSceneWithLoadingScreen(gallerySceneIdx));
        }
        else
        {
            Debug.LogError($"Instance of {nameof(SceneTransitionManager)} not found!");
        }
    }
}
