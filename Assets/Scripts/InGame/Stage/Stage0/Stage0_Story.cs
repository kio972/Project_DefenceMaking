using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Stage0_Story : MonoBehaviour
{
    private readonly string _Entry0 = "Tuto000";
    private readonly string _Entry1 = "Tuto001";
    //private readonly string _Entry2 = "Tuto002";

    private readonly string _Tuto0 = "Tuto100";
    //private readonly string _Tuto1 = "Tuto101";
    private readonly string _Tuto2_a = "Tuto102_0";
    private readonly string _Tuto2_b = "Tuto102_1";
    private readonly string _Tuto3 = "Tuto103";
    //private readonly string _Tuto4 = "Tuto104";
    private readonly string _Tuto5 = "Tuto105";
    private readonly string _Tuto6 = "Tuto106";
    //private readonly string _Tuto7 = "Tuto107";
    private readonly string _Tuto8 = "Tuto108";
    private readonly string _Tuto9 = "Tuto109";
    //private readonly string _Tuto10 = "Tuto110";

    
    

    public int tutoProgress { get; private set; }


    [SerializeField]
    private GameObject deckBtn;
    [SerializeField]
    private GameObject manaUI;
    [SerializeField]
    private GameObject speedUI;
    [SerializeField]
    private GameObject shopBtn;

    private bool LockDestroyTile(ITileKind tileKind)
    {
        if(tileKind is Tile tile)
        {
            tile.IsRemovable = false;
            return true;
        }

        return false;
    }

    private async UniTask PlayScript(string targetScript)
    {
        StoryManager.Instance.EnqueueScript(targetScript);
        await UniTask.WaitUntil(() => StoryManager.Instance.IsScriptQueueEmpty, cancellationToken: gameObject.GetCancellationTokenOnDestroy());
    }

    private async UniTaskVoid DeckUIQuestCheck()
    {
        const string targetQuest = "q0105";
        await UniTask.WaitUntil(() => QuestManager.Instance.questController.subQuest.Where(_ => _._QuestID == targetQuest).Count() >= 1 || QuestManager.Instance.IsQuestEnded(targetQuest), cancellationToken: gameObject.GetCancellationTokenOnDestroy());
        deckBtn.SetActive(true);
    }

    private async UniTaskVoid ManaUIQuestCheck()
    {
        const string targetQuest = "q0102";
        await UniTask.WaitUntil(() => QuestManager.Instance.questController.subQuest.Where(_ => _._QuestID == targetQuest).Count() >= 1 || QuestManager.Instance.IsQuestEnded(targetQuest), cancellationToken: gameObject.GetCancellationTokenOnDestroy());
        manaUI.SetActive(true);
    }

    private async UniTaskVoid SpeedUIQuestCheck()
    {
        GameManager.Instance.speedLock = true;
        const string targetQuest = "q0103";
        await UniTask.WaitUntil(() => QuestManager.Instance.questController.subQuest.Where(_ => _._QuestID == targetQuest).Count() >= 1 || QuestManager.Instance.IsQuestEnded(targetQuest), cancellationToken: gameObject.GetCancellationTokenOnDestroy());
        speedUI.SetActive(true);
        GameManager.Instance.speedLock = false;
    }

    private async UniTaskVoid ShopUIQuestCheck()
    {
        const string targetQuest = "q0106";
        await UniTask.WaitUntil(() => QuestManager.Instance.questController.subQuest.Where(_ => _._QuestID == targetQuest).Count() >= 1 || QuestManager.Instance.IsQuestEnded(targetQuest), cancellationToken: gameObject.GetCancellationTokenOnDestroy());
        shopBtn.SetActive(true);
    }

    // Start is called before the first frame update
    async UniTaskVoid Start()
    {
        if (SaveManager.Instance.playerData != null)
        {
            GameManager.Instance.LoadGame(SaveManager.Instance.playerData);
            GameManager.Instance.speedController.SetSpeedNormal();
            DeckUIQuestCheck().Forget();
            ManaUIQuestCheck().Forget();
            SpeedUIQuestCheck().Forget();
            ShopUIQuestCheck().Forget();
            return;
        }

        if (SettingManager.Instance.stageState == 0)
            await PlayScript(_Entry0);
        else
            await PlayScript(_Entry1);

        await UniTask.Yield(cancellationToken: gameObject.GetCancellationTokenOnDestroy());

        NodeManager.Instance.AddSetTileEvent(LockDestroyTile);
        
        GameManager.Instance.Init();
        GameManager.Instance.speedController.SetSpeedZero();
        GameManager.Instance.timerLock = true;
        GameManager.Instance.spawnLock = true;
        GameManager.Instance.drawLock = true;
        GameManager.Instance.speedLock = true;
        GameManager.Instance.cameraLock = true;
        GameManager.Instance.saveLock = true;
        GameManager.Instance.moveLock = true;

        Vector3 camZeroPos = GameManager.Instance.cameraController.guidePos;

        //튜토리얼 카드 추가
        int directPath = DataManager.Instance.deckListIndex["c10001"];
        for (int i = 0; i < 4; i++)
            GameManager.Instance.cardDeckController.AddCard(directPath);
        int crossRoad = DataManager.Instance.deckListIndex["c10007"];
        GameManager.Instance.cardDeckController.AddCard(crossRoad);
        
        int road2 = DataManager.Instance.deckListIndex["c10003"];
        int road4 = DataManager.Instance.deckListIndex["c10005"];
        //int road8 = DataManager.Instance.deckListIndex["c10009"];
        //int road10 = DataManager.Instance.deckListIndex["c10011"];
        GameManager.Instance.cardDeckController.AddCard(road2);
        GameManager.Instance.cardDeckController.AddCard(road4);
        //GameManager.Instance.cardDeckController.AddCard(road8);
        //GameManager.Instance.cardDeckController.AddCard(road10);
        //int room0 = DataManager.Instance.deckListIndex["c11001"];
        int room1 = DataManager.Instance.deckListIndex["c11002"];
        //int room2 = DataManager.Instance.deckListIndex["c11003"];
        //GameManager.Instance.cardDeckController.AddCard(room0);
        for(int i = 0; i < 2; i++)
            GameManager.Instance.cardDeckController.AddCard(room1);
        //GameManager.Instance.cardDeckController.AddCard(room2);


        await UniTask.WaitForSeconds(2.5f, cancellationToken: gameObject.GetCancellationTokenOnDestroy());
        await PlayScript(_Tuto0);
        tutoProgress = 0;


        //GameManager.Instance.cameraController.SetCamZoom(1);
        //GameManager.Instance.cameraController.CamMoveToPos(camZeroPos);

        //await UniTask.WaitForSeconds(1, cancellationToken: gameObject.GetCancellationTokenOnDestroy());
        //await PlayScript(_Tuto1);

        tutoProgress = 1;

        GameManager.Instance.rotateLock = true;

        for (int i = 0; i < 3; i++)
            GameManager.Instance.cardDeckController.DrawCard(directPath);

        await UniTask.WaitUntil(() => GameManager.Instance.cardDeckController.hand_CardNumber == 0, cancellationToken: gameObject.GetCancellationTokenOnDestroy());
        await PlayScript(_Tuto2_a);

        tutoProgress = 2;

        GameManager.Instance.rotateLock = false;
        GameManager.Instance.cardDeckController.DrawCard(crossRoad);

        await UniTask.WaitUntil(() => GameManager.Instance.cardDeckController.hand_CardNumber == 0, cancellationToken: gameObject.GetCancellationTokenOnDestroy());

        await UniTask.WaitForSeconds(1, cancellationToken: gameObject.GetCancellationTokenOnDestroy());
        await PlayScript(_Tuto2_b);


        await UniTask.WaitForSeconds(1, cancellationToken: gameObject.GetCancellationTokenOnDestroy());
        BattlerPooling.Instance.SpawnAdventurer("farmer_sickle");
        GameManager.Instance.cameraController.SetCamZoom(4);
        GameManager.Instance.cameraController.ResetCamPos(true);

        await UniTask.WaitForSeconds(1, cancellationToken: gameObject.GetCancellationTokenOnDestroy());
        await PlayScript(_Tuto3);
        //await UniTask.WaitForSeconds(1, cancellationToken: gameObject.GetCancellationTokenOnDestroy());
        //await PlayScript(_Tuto4);

        tutoProgress = 3;
        
        GameManager.Instance.cameraController.SetCamZoom(1);
        GameManager.Instance.cameraController.CamMoveToPos(camZeroPos);

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

        //await PlayScript(_Tuto7);

        tutoProgress = 4;

        await UniTask.WaitUntil(() => GameManager.Instance.research.curResearch != null && !GameManager.Instance.isPause, cancellationToken: gameObject.GetCancellationTokenOnDestroy());

        tutoProgress = 5;

        GameManager.Instance.speedLock = false;
        GameManager.Instance.speedController.SetSpeedNormal();
        GameManager.Instance.speedLock = true;

        await UniTask.WaitUntil(() => GameManager.Instance.research.completedResearchs.Contains("r_m10001"), cancellationToken: gameObject.GetCancellationTokenOnDestroy());
        await UniTask.WaitForSeconds(1, cancellationToken: gameObject.GetCancellationTokenOnDestroy());
        await PlayScript(_Tuto8);

        tutoProgress = 6;

        GameManager.Instance.cardDeckController.DrawCard(room1);

        await UniTask.WaitUntil(() => GameManager.Instance.cardDeckController.hand_CardNumber == 0, cancellationToken: gameObject.GetCancellationTokenOnDestroy());

        tutoProgress = 7;
        await UniTask.WaitUntil(() => GameManager.Instance.monsterSpawner.Count > 0, cancellationToken: gameObject.GetCancellationTokenOnDestroy());

        tutoProgress = 8;

        await UniTask.WaitForSeconds(1, cancellationToken: gameObject.GetCancellationTokenOnDestroy());
        await PlayScript(_Tuto9);
        
        //await PlayScript(_Tuto10);

        tutoProgress = 10;

        GameManager.Instance.timerLock = false;
        GameManager.Instance.spawnLock = false;
        GameManager.Instance.drawLock = false;
        //GameManager.Instance.speedLock = false;
        GameManager.Instance.cameraLock = false;
        GameManager.Instance.saveLock = false;
        GameManager.Instance.moveLock = false;
        GameManager.Instance.speedController.SetSpeedNormal();

        NodeManager.Instance.RemoveSetTileEvent(LockDestroyTile);

        //시작패 드로우
        GameManager.Instance.cardDeckController.DrawCard(directPath);
        GameManager.Instance.cardDeckController.DrawCard(road2);
        GameManager.Instance.cardDeckController.DrawCard(road4);
        //GameManager.Instance.cardDeckController.DrawCard(road8);
        //GameManager.Instance.cardDeckController.DrawCard(road10);
        //GameManager.Instance.cardDeckController.DrawCard(room0);
        GameManager.Instance.cardDeckController.DrawCard(room1);
        //GameManager.Instance.cardDeckController.DrawCard(room2);

        await UniTask.WaitForSeconds(1, cancellationToken: gameObject.GetCancellationTokenOnDestroy());
        //GameManager.Instance.waveController.SpawnWave(0);
        QuestManager.Instance.InitQuest();

        SaveManager.Instance.SavePlayerData();

        DeckUIQuestCheck().Forget();
        ManaUIQuestCheck().Forget();
        SpeedUIQuestCheck().Forget();
        ShopUIQuestCheck().Forget();
    }
}
