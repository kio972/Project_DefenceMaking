using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : Singleton<SceneController>
{
    public float sceneWaitTime = 1f;
    private bool sceneChanging = false;
    public bool SceneChanging { get { return sceneChanging; } }

    private IEnumerator IMoveScene(string sceneName, float time)
    {
        sceneChanging = true;
        float elapsed = 0f;
        UIFade.Instance.FadeOut(time);
        while (elapsed < time)
        {
            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }

        SceneManager.LoadSceneAsync(sceneName);
        yield return null;

        elapsed = 0f;
        while (elapsed < sceneWaitTime)
        {
            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }

        UIFade.Instance.FadeIn(time);
        elapsed = 0f;
        while (elapsed < time)
        {
            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }

        sceneChanging = false;
    }

    public void MoveScene(string sceneName, float time = 1f)
    {
        if (sceneChanging)
            return;
        StartCoroutine(IMoveScene(sceneName, time));
        UIManager.Instance.ResetUI();
    }

    public void RestartScene(float time = 1f)
    {
        MoveScene(SceneManager.GetActiveScene().name, time);
    }
}
