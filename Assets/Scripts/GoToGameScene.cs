using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoToGameScene : MonoBehaviour
{
    protected virtual void MoveScene()
    {
        SceneController.Instance.MoveScene("MapScene");
    }

    private async UniTaskVoid Start()
    {
        await UniTask.WaitUntil(() => !SceneController.Instance.SceneChanging);
        MoveScene();
    }
}
