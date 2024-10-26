using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage0_Story : MonoBehaviour
{
    private readonly string _Entry0 = "Tuto000";
    private readonly string _Entry1 = "Tuto001";
    private readonly string _Entry2 = "Tuto002";

    private readonly string _Tuto0 = "Tuto100";
    private readonly string _Tuto1 = "Tuto101";
    private readonly string _Tuto2 = "Tuto102";
    private readonly string _Tuto3 = "Tuto103";
    private readonly string _Tuto4 = "Tuto104";
    private readonly string _Tuto5 = "Tuto105";
    private readonly string _Tuto6 = "Tuto106";
    private readonly string _Tuto7 = "Tuto107";
    private readonly string _Tuto8 = "Tuto108";
    private readonly string _Tuto9 = "Tuto109";
    private readonly string _Tuto10 = "Tuto110";

    private async UniTask PlayScript(string targetScript)
    {
        StoryManager.Instance.EnqueueScript(targetScript);
        await UniTask.WaitUntil(() => StoryManager.Instance.IsScriptQueueEmpty, cancellationToken: gameObject.GetCancellationTokenOnDestroy());
    }



    // Start is called before the first frame update
    async UniTaskVoid Start()
    {
        GameManager.Instance.Init();

        //StoryManager.Instance.triggerZone = this;
        //if (SettingManager.Instance.stageState > 0)
        //    await PlayScript(_Entry0);
        //else
        //    await PlayScript(_Entry1);

        //await PlayScript(_Entry2);

        //await PlayScript(_Tuto0);

        ////GameManager.Instance.cameraController.ResetCamPos();

        //await PlayScript(_Tuto1);


        //await PlayScript(_Tuto2);
        //await PlayScript(_Tuto3);
        //await PlayScript(_Tuto4);
        //await PlayScript(_Tuto5);
        //await PlayScript(_Tuto6);
        //await PlayScript(_Tuto7);
        //await PlayScript(_Tuto8);
        //await PlayScript(_Tuto9);
        //await PlayScript(_Tuto10);
    }
}
