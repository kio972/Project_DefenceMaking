using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoToGameScene : MonoBehaviour
{
    public float fakeLoadingTime = 3f;
    protected virtual void MoveScene()
    {
        string targetScene = string.IsNullOrEmpty(SettingManager.Instance.nextScene) ? "Stage0" : SettingManager.Instance.nextScene;
        
        //if (SaveManager.Instance.playerData != null)
        //    targetScene = SaveManager.Instance.playerData.sceneName;

        SceneController.Instance.MoveScene(targetScene);
    }

    private async UniTaskVoid Start()
    {
        AudioManager.Instance.StopBGM(5000);
        float waitTime = Time.time;
        await UniTask.WaitUntil(() => !SceneController.Instance.SceneChanging);
        waitTime -= Time.time;
        waitTime = fakeLoadingTime - waitTime;

        await UniTask.Delay(System.TimeSpan.FromSeconds(waitTime));

        MoveScene();
    }
}
