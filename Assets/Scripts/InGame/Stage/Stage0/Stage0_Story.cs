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

        //튜토리얼 카드 추가
        int directPath = DataManager.Instance.deckListIndex["c10001"];
        for (int i = 0; i < 4; i++)
            GameManager.Instance.cardDeckController.AddCard(directPath);
        int crossRoad = DataManager.Instance.deckListIndex["c10007"];
        GameManager.Instance.cardDeckController.AddCard(crossRoad);
        
        int road2 = DataManager.Instance.deckListIndex["c10003"];
        int road4 = DataManager.Instance.deckListIndex["c10005"];
        int road8 = DataManager.Instance.deckListIndex["c10009"];
        int road10 = DataManager.Instance.deckListIndex["c10011"];
        GameManager.Instance.cardDeckController.AddCard(road2);
        GameManager.Instance.cardDeckController.AddCard(road4);
        GameManager.Instance.cardDeckController.AddCard(road8);
        GameManager.Instance.cardDeckController.AddCard(road10);
        int room0 = DataManager.Instance.deckListIndex["c11001"];
        int room1 = DataManager.Instance.deckListIndex["c11002"];
        int room2 = DataManager.Instance.deckListIndex["c11003"];
        GameManager.Instance.cardDeckController.AddCard(room0);
        GameManager.Instance.cardDeckController.AddCard(room1);
        GameManager.Instance.cardDeckController.AddCard(room2);


        await UniTask.WaitForSeconds(2.5f, cancellationToken: gameObject.GetCancellationTokenOnDestroy());
        await PlayScript(_Tuto0);
        await UniTask.WaitForSeconds(1, cancellationToken: gameObject.GetCancellationTokenOnDestroy());
        await PlayScript(_Tuto1);
        
        
        for (int i = 0; i < 3; i++)
            GameManager.Instance.cardDeckController.DrawCard(directPath);

        await UniTask.WaitUntil(() => GameManager.Instance.cardDeckController.hand_CardNumber == 0, cancellationToken: gameObject.GetCancellationTokenOnDestroy());
        await PlayScript(_Tuto2);

        //StoryManager.Instance.triggerZone = this;
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
        await UniTask.WaitUntil(() => GameManager.Instance.trapList.Count > 0, cancellationToken: gameObject.GetCancellationTokenOnDestroy());
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


        await UniTask.WaitUntil(() => GameManager.Instance.research.curResearch != null && !GameManager.Instance.isPause, cancellationToken: gameObject.GetCancellationTokenOnDestroy());

        GameManager.Instance.speedLock = false;
        GameManager.Instance.speedController.SetSpeedNormal();
        GameManager.Instance.speedLock = true;

        await UniTask.WaitUntil(() => GameManager.Instance.research.completedResearchs.Contains("r_m10001"), cancellationToken: gameObject.GetCancellationTokenOnDestroy());
        await UniTask.WaitForSeconds(1, cancellationToken: gameObject.GetCancellationTokenOnDestroy());
        await PlayScript(_Tuto8);

        GameManager.Instance.cardDeckController.DrawCard(room1);

        await UniTask.WaitUntil(() => GameManager.Instance.cardDeckController.hand_CardNumber == 0, cancellationToken: gameObject.GetCancellationTokenOnDestroy());
        deployBtn.SetActive(true);
        await UniTask.WaitUntil(() => GameManager.Instance.monsterSpawner.Count > 0, cancellationToken: gameObject.GetCancellationTokenOnDestroy());
        
        await UniTask.WaitForSeconds(1, cancellationToken: gameObject.GetCancellationTokenOnDestroy());
        await PlayScript(_Tuto9);
        
        await PlayScript(_Tuto10);

        GameManager.Instance.timerLock = false;
        GameManager.Instance.spawnLock = false;
        GameManager.Instance.drawLock = false;
        GameManager.Instance.speedLock = false;
        GameManager.Instance.speedController.SetSpeedNormal();

        //시작패 드로우
        GameManager.Instance.cardDeckController.DrawCard(directPath);
        GameManager.Instance.cardDeckController.DrawCard(road2);
        GameManager.Instance.cardDeckController.DrawCard(road4);
        GameManager.Instance.cardDeckController.DrawCard(road8);
        GameManager.Instance.cardDeckController.DrawCard(road10);
        GameManager.Instance.cardDeckController.DrawCard(room0);
        GameManager.Instance.cardDeckController.DrawCard(room1);
        GameManager.Instance.cardDeckController.DrawCard(room2);

        await UniTask.WaitForSeconds(1, cancellationToken: gameObject.GetCancellationTokenOnDestroy());
        //GameManager.Instance.waveController.SpawnWave(0);
        QuestManager.Instance.InitQuest();
    }
}
