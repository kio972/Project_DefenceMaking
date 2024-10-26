using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoToGameScene : MonoBehaviour
{
    public float fakeLoadingTime = 3f;
    protected virtual void MoveScene()
    {
        SceneController.Instance.MoveScene("MapScene");
    }

    private async UniTaskVoid Start()
    {
        float waitTime = Time.time;
        await UniTask.WaitUntil(() => !SceneController.Instance.SceneChanging);
        waitTime -= Time.time;
        waitTime = fakeLoadingTime - waitTime;

        await UniTask.Delay(System.TimeSpan.FromSeconds(waitTime));

        MoveScene();
    }
}
