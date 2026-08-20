using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class LoadingScreen : SingletonMonoBehaviour<LoadingScreen>
{
    [SerializeField] private float fadeInTime = 1f;
    [SerializeField] private float fadeOutTime = 1f;
    [SerializeField] private GameObject loadingObjectRoot;
    [SerializeField] private Image progressBar;

    //progress should be between 0 and 1
    public void SetProgressBar(float progress)
    {
        progressBar.fillAmount = Mathf.Clamp01(progress);
    }

    public async Task FadeInRoutine()
    {
        loadingObjectRoot.SetActive(true);
        await Task.Delay((int)fadeInTime * 1000);
    }

    public async Task FadeOutRoutine()
    {
        await Task.Delay((int)fadeOutTime * 1000);
        loadingObjectRoot.SetActive(false);
    }
}
