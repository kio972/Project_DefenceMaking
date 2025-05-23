using Cysharp.Threading.Tasks;
using FMOD.Studio;
using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

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
    private readonly string _Tuto10 = "Tuto110";
    private readonly string _Tuto11 = "Tuto111";
    private readonly string _Tuto12 = "Tuto112";


    public int tutoProgress { get; private set; }


    [SerializeField]
    private GameObject deckBtn;
    [SerializeField]
    private GameObject manaUI;
    [SerializeField]
    private GameObject speedUI;
    [SerializeField]
    private GameObject shopBtn;

    [SerializeField]
    private CardSelection cardSelection;

    [SerializeField]
    private EventSystem eventSystem;

    [SerializeField]
    FMODUnity.EventReference bgmSound;

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

    private async UniTaskVoid CardSelecttionCheck()
    {
        await UniTask.WaitUntil(() => cardSelection.gameObject.activeInHierarchy, cancellationToken: gameObject.GetCancellationTokenOnDestroy());
        eventSystem.enabled = false;

        await UniTask.WaitForSeconds(2, cancellationToken: gameObject.GetCancellationTokenOnDestroy());
        await PlayScript(_Tuto12);
        eventSystem.enabled = true;
    }

    // Start is called before the first frame update
    async UniTaskVoid Start()
    {
        if (SaveManager.Instance.playerData != null)
        {
            GameManager.Instance.LoadGame(SaveManager.Instance.playerData);
            GameManager.Instance.speedController.SetSpeedNormal();
            DeckUIQuestCheck().Forget();
            //ManaUIQuestCheck().Forget();
            manaUI.SetActive(true);
            SpeedUIQuestCheck().Forget();
            //ShopUIQuestCheck().Forget();
            CardSelecttionCheck().Forget();
            NodeManager.Instance.AddSetTileEvent(LockDestroyTile);
            AudioManager.Instance.PlayBackground(bgmSound);
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
        //for (int i = 0; i < 4; i++)
        //    GameManager.Instance.cardDeckController.AddCard(directPath);
        int crossRoad = DataManager.Instance.deckListIndex["c10007"];
        //GameManager.Instance.cardDeckController.AddCard(crossRoad);
        
        int road6 = DataManager.Instance.deckListIndex["c10007"];
        int road4 = DataManager.Instance.deckListIndex["c10005"];
        //int road8 = DataManager.Instance.deckListIndex["c10009"];
        //int road10 = DataManager.Instance.deckListIndex["c10011"];
        //GameManager.Instance.cardDeckController.AddCard(road6);
        //GameManager.Instance.cardDeckController.AddCard(road4);
        //GameManager.Instance.cardDeckController.AddCard(road8);
        //GameManager.Instance.cardDeckController.AddCard(road10);
        //int room0 = DataManager.Instance.deckListIndex["c11001"];
        int room1 = DataManager.Instance.deckListIndex["c11002"];

        int roomPart = DataManager.Instance.deckListIndex["c12012"];
        int doorPart = DataManager.Instance.deckListIndex["c13007"];
        //int room2 = DataManager.Instance.deckListIndex["c11003"];
        //GameManager.Instance.cardDeckController.AddCard(room0);
        //for (int i = 0; i < 2; i++)
        //    GameManager.Instance.cardDeckController.AddCard(room1);
        //GameManager.Instance.cardDeckController.AddCard(room2);

        AudioManager.Instance.PlayBackground(bgmSound);

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

        GameManager.Instance.speedLock = false;
        GameManager.Instance.speedController.SetSpeedZero();
        GameManager.Instance.speedLock = true;

        await PlayScript(_Tuto10);

        GameManager.Instance.cameraController.CamMoveToPos(NodeManager.Instance.FindNode(-3, 6).transform.position);
        await UniTask.WaitForSeconds(0.5f, cancellationToken: gameObject.GetCancellationTokenOnDestroy());
        manaUI.SetActive(true);
        if (GameManager.Instance.mapBuilder is Stage0_mapBuilder tutoBuilder)
        {
            TileNode door = tutoBuilder.SetExampleRoom();
            door.isLock = true;

            await UniTask.WaitForSeconds(1.5f, cancellationToken: gameObject.GetCancellationTokenOnDestroy());
            GameManager.Instance.cardDeckController.DrawCard(roomPart);
            tutoProgress = 9;

            await UniTask.WaitUntil(() => GameManager.Instance.cardDeckController.hand_CardNumber == 0, cancellationToken: gameObject.GetCancellationTokenOnDestroy());
            NodeManager.Instance.SetActiveNode(door, false);
            GameManager.Instance.cardDeckController.DrawCard(doorPart);

            tutoProgress = 10;

            await UniTask.WaitForSeconds(1, cancellationToken: gameObject.GetCancellationTokenOnDestroy());
            await UniTask.WaitUntil(() => GameManager.Instance.cardDeckController.hand_CardNumber == 0, cancellationToken: gameObject.GetCancellationTokenOnDestroy());

            tutoProgress = 11;

            await UniTask.WaitUntil(() => GameManager.Instance.monsterSpawner.Count > 1, cancellationToken: gameObject.GetCancellationTokenOnDestroy());
            door.isLock = false;
            NodeManager.Instance.SetActiveNode(door, true);
        }

        GameManager.Instance.cameraController.CamMoveToPos(GameManager.Instance.monsterSpawner[1].transform.position);
        tutoProgress = 12;

        await PlayScript(_Tuto11);
        GameManager.Instance.cameraController.CamMoveToPos(camZeroPos);

        tutoProgress = 20;
        GameManager.Instance.timerLock = false;
        GameManager.Instance.spawnLock = false;
        GameManager.Instance.drawLock = false;
        //GameManager.Instance.speedLock = false;
        GameManager.Instance.cameraLock = false;
        GameManager.Instance.saveLock = false;
        GameManager.Instance.moveLock = false;
        GameManager.Instance.speedController.SetSpeedNormal();

        //NodeManager.Instance.RemoveSetTileEvent(LockDestroyTile);

        //시작패 드로우
        GameManager.Instance.cardDeckController.DrawCard(directPath);
        GameManager.Instance.cardDeckController.DrawCard(road4);
        GameManager.Instance.cardDeckController.DrawCard(road6);
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
        //ManaUIQuestCheck().Forget();
        SpeedUIQuestCheck().Forget();
        //ShopUIQuestCheck().Forget();
        CardSelecttionCheck().Forget();
    }
}
