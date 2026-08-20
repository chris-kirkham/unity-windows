using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

//N.B. this must be in the right place to receive player input!
public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenuRoot;
    [SerializeField] private Button resumeButton;
    [SerializeField] private SceneTransitionButton mainMenuButton;

    private void OnEnable()
    {
        PauseGame();

        if(resumeButton)
        {
            resumeButton.onClick.AddListener(ResumeGame);
        }

        if(mainMenuButton)
        {
            mainMenuButton.onClick += OnMainMenuButtonClicked;
        }
    }

    private void OnDisable()
    {
        if(resumeButton)
        {
            resumeButton.onClick.RemoveAllListeners();
        }

        if(mainMenuButton)
        {
            mainMenuButton.onClick -= OnMainMenuButtonClicked;
        }
    }

    private void PauseGame()
    {
        if(!enabled)
        {
            OnEnable();
        }

        Time.timeScale = 0f;
    }

    private void ResumeGame()
    {
        Time.timeScale = 1f;
        pauseMenuRoot.SetActive(false);
    }

    private void OnMainMenuButtonClicked()
    {
        //TODO: the SceneTransitionButton will change scenes after this callback - do anything needed to close the current scene here! (how?)
        ResumeGame(); 
    }

#region InputActions
    private void OnPauseGame(InputValue value)
    {
        if(enabled)
        {
            ResumeGame();
        }
        else
        {
            PauseGame();
        }
    }
#endregion
}
