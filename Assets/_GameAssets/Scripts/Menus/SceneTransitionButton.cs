using System;
using UnityEngine;
using UnityEngine.UI;

public class SceneTransitionButton : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private SceneBuildIndex targetScene;
    [SerializeField] private bool useLoadingScreen = true;

    public event Action onClick;

    private void OnEnable()
    {
        button.onClick.AddListener(OnClick);
    }

    private void OnDisable()
    {
        button.onClick.RemoveAllListeners();
    }

    private void OnClick()
    {
        onClick?.Invoke();

        SceneTransitionManager.Inst.LoadScene((int)targetScene, useLoadingScreen);
    }
}
