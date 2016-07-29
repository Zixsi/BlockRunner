using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UILoaderWindow : UIWindow
{
    [HideInInspector]
    public string sceneName = null;

    // Слайдер прогресс бара
    public Slider progressBar;
    // Прогресс загрузки
    private AsyncOperation _loadProgress;

    public override void Show()
    {
        base.Show();
        StartCoroutine(StartLoad());
    }

    IEnumerator StartLoad()
    {
        yield return new WaitForSeconds(1.0f);

        if(sceneName != null)
            _loadProgress = SceneManager.LoadSceneAsync(sceneName);

        if(_loadProgress != null)
        {
            while(!_loadProgress.isDone)
            {
                if(_loadProgress.progress > progressBar.value)
                    progressBar.value = _loadProgress.progress;
                yield return null;
            }
        }
    }
}
