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

    [SerializeField]
    private GameObject deployBtn;
    [SerializeField]
    private GameObject researchBtn;
    [SerializeField]
    private GameObject shopBtn;

    private async UniTask PlayScript(string targetScript)
    {
        StoryManager.Instance.EnqueueScript(targetScript);
        await UniTask.WaitUntil(() => StoryManager.Instance.IsScriptQueueEmpty, cancellationToken: gameObject.GetCancellationTokenOnDestroy());
    }



    // Start is called before the first frame update
    async UniTaskVoid Start()
    {
        if (SettingManager.Instance.stageState == 0)
            await PlayScript(_Entry0);
        else
            await PlayScript(_Entry1);

        await PlayScript(_Entry2);

        
        GameManager.Instance.Init();
        GameManager.Instance.speedController.SetSpeedZero();
        GameManager.Instance.timerLock = true;
        GameManager.Instance.spawnLock = true;
        GameManager.Instance.drawLock = true;
        GameManager.Instance.speedLock = true;

        await UniTask.WaitForSeconds(2.5f, cancellationToken: gameObject.GetCancellationTokenOnDestroy());
        await PlayScript(_Tuto0);
        await UniTask.WaitForSeconds(1, cancellationToken: gameObject.GetCancellationTokenOnDestroy());
        await PlayScript(_Tuto1);
        
        int directPath = DataManager.Instance.deckListIndex["c10001"];
        for(int i = 0; i < 3; i++)
        {
            GameManager.Instance.cardDeckController.AddCard(directPath);
            GameManager.Instance.cardDeckController.DrawCard(directPath);
        }

        await UniTask.WaitUntil(() => GameManager.Instance.cardDeckController.hand_CardNumber == 0, cancellationToken: gameObject.GetCancellationTokenOnDestroy());
        await PlayScript(_Tuto2);

        //StoryManager.Instance.triggerZone = this;
        int crossRoad = DataManager.Instance.deckListIndex["c10007"];
        GameManager.Instance.cardDeckController.AddCard(crossRoad);
        GameManager.Instance.cardDeckController.DrawCard(crossRoad);

        await UniTask.WaitUntil(() => GameManager.Instance.cardDeckController.hand_CardNumber == 0, cancellationToken: gameObject.GetCancellationTokenOnDestroy());

        await UniTask.WaitForSeconds(1, cancellationToken: gameObject.GetCancellationTokenOnDestroy());
        BattlerPooling.Instance.SpawnAdventurer("farmer_sickle");
        await UniTask.WaitForSeconds(1, cancellationToken: gameObject.GetCancellationTokenOnDestroy());

        await PlayScript(_Tuto3);
        await UniTask.WaitForSeconds(1, cancellationToken: gameObject.GetCancellationTokenOnDestroy());
        await PlayScript(_Tuto4);

        deployBtn.SetActive(true);

        ////GameManager.Instance.cameraController.ResetCamPos();
        await UniTask.WaitUntil(() => GameManager.Instance.gold < 50, cancellationToken: gameObject.GetCancellationTokenOnDestroy());
        await UniTask.WaitForSeconds(1, cancellationToken: gameObject.GetCancellationTokenOnDestroy());
        await PlayScript(_Tuto5);

        GameManager.Instance.speedLock = false;
        GameManager.Instance.speedController.SetSpeedNormal();
        GameManager.Instance.speedLock = true;

        await UniTask.WaitUntil(() => GameManager.Instance.adventurersList.Count == 0, cancellationToken: gameObject.GetCancellationTokenOnDestroy());

        GameManager.Instance.speedLock = false;
        GameManager.Instance.speedController.SetSpeedZero();
        GameManager.Instance.speedLock = true;

        await UniTask.WaitForSeconds(1, cancellationToken: gameObject.GetCancellationTokenOnDestroy());
        await PlayScript(_Tuto6);

        await PlayScript(_Tuto7);
        researchBtn.SetActive(true);
        await UniTask.WaitUntil(() => GameManager.Instance.gold < 50, cancellationToken: gameObject.GetCancellationTokenOnDestroy());

        GameManager.Instance.speedLock = false;
        GameManager.Instance.speedController.SetSpeedNormal();
        GameManager.Instance.speedLock = true;

        await UniTask.WaitUntil(() => GameManager.Instance.research.completedResearchs.Count >= 1, cancellationToken: gameObject.GetCancellationTokenOnDestroy());
        await UniTask.WaitForSeconds(1, cancellationToken: gameObject.GetCancellationTokenOnDestroy());
        await PlayScript(_Tuto8);

        int room = DataManager.Instance.deckListIndex["c11002"];
        GameManager.Instance.cardDeckController.AddCard(room);
        GameManager.Instance.cardDeckController.DrawCard(room);

        await UniTask.WaitUntil(() => GameManager.Instance.adventurersList.Count == 0, cancellationToken: gameObject.GetCancellationTokenOnDestroy());
        deployBtn.SetActive(true);
        await UniTask.WaitUntil(() => GameManager.Instance.gold < 50, cancellationToken: gameObject.GetCancellationTokenOnDestroy());
        
        await UniTask.WaitForSeconds(1, cancellationToken: gameObject.GetCancellationTokenOnDestroy());
        await PlayScript(_Tuto9);
        
        await PlayScript(_Tuto10);

        GameManager.Instance.timerLock = false;
        GameManager.Instance.spawnLock = false;
        GameManager.Instance.drawLock = false;
        GameManager.Instance.speedLock = false;
        GameManager.Instance.speedController.SetSpeedNormal();

        //시작패 드로우

        await UniTask.WaitForSeconds(1, cancellationToken: gameObject.GetCancellationTokenOnDestroy());
        GameManager.Instance.waveController.SpawnWave(0);
        QuestManager.Instance.InitQuest();
    }
}
