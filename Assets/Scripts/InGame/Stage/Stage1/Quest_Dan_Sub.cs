using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UniRx;
using UnityEngine;

public class QuestKillBattler : Quest
{
    protected string targetId;

    private bool isInit = false;

    protected virtual void CheckBattlerDead(Battler battler, Battler attecker)
    {
        if (battler.BattlerID != targetId)
            return;

        curClearNum[0]++;
    }

    public override void CheckCondition()
    {
        if (!isInit)
        {
            GameManager.Instance.AddBattlerDeadEvent(CheckBattlerDead);
            isInit = true;
        }

        if (curClearNum[0] >= Mathf.Abs(_ClearNum[0]))
            isComplete[0] = true;
    }

    public override void FailQuest()
    {
        base.FailQuest();
        GameManager.Instance.RemoveBattlerDeadEvent(CheckBattlerDead);
    }

    public override void CompleteQuest()
    {
        base.CompleteQuest();
        GameManager.Instance.RemoveBattlerDeadEvent(CheckBattlerDead);
    }
}


public abstract class SetTileQuest : Quest
{
    protected abstract bool SetTileCheck(ITileKind tileKind);

    protected override void InitEvent()
    {
        NodeManager.Instance.AddSetTileEvent(SetTileCheck);
    }

    public override void FailQuest()
    {
        NodeManager.Instance.RemoveSetTileEvent(SetTileCheck);
        base.FailQuest();
    }

    public override void CompleteQuest()
    {
        NodeManager.Instance.RemoveSetTileEvent(SetTileCheck);
        base.CompleteQuest();
    }
}

public abstract class RemoveTileQuest : Quest
{
    protected abstract bool RemoveTileCheck(ITileKind tileKind);

    protected override void InitEvent()
    {
        NodeManager.Instance.AddRemoveTileEvent(RemoveTileCheck);
    }

    public override void FailQuest()
    {
        NodeManager.Instance.RemoveRemoveTileEvent(RemoveTileCheck);
        base.FailQuest();
    }

    public override void CompleteQuest()
    {
        NodeManager.Instance.RemoveRemoveTileEvent(RemoveTileCheck);
        base.CompleteQuest();
    }
}

public abstract class ShopBuyQuest : Quest
{
    protected abstract void CheckBuy(Item item);

    protected override void InitEvent()
    {
        GameManager.Instance.shop.AddBuyItemEvent(CheckBuy);
    }

    public override void FailQuest()
    {
        GameManager.Instance.shop.AddBuyItemEvent(CheckBuy);
        base.FailQuest();
    }

    public override void CompleteQuest()
    {
        GameManager.Instance.shop.AddBuyItemEvent(CheckBuy);
        base.CompleteQuest();
    }
}

public abstract class QuestUniRx : Quest
{
    protected CancellationTokenSource cancellationToken = new CancellationTokenSource();
    public override void FailQuest()
    {
        base.FailQuest();
        cancellationToken.Cancel();
        cancellationToken.Dispose();
    }

    public override void CompleteQuest()
    {
        base.CompleteQuest();
        cancellationToken.Cancel();
        cancellationToken.Dispose();
    }
}

public class Quest2001 : Quest
{
    //카메라 이동 퀘스트
    Vector3 prevPos = Vector3.zero;

    public override void CheckCondition()
    {
        if (prevPos == Vector3.zero)
            prevPos = GameManager.Instance.cameraController.transform.position;

        if ((GameManager.Instance.cameraController.transform.position - prevPos).magnitude > 0.1f)
        {
            curClearNum[0]++;
            isComplete[0] = true;
        }
    }
}

public class Quest2002 : Quest
{
    //허브 타일 설치 퀘스트
    private bool isInit;

    private bool HerbTileCheck(ITileKind tileKind)
    {
        if (tileKind is not HerbProduction)
            return false;

        curClearNum[0]++;
        return true;
    }

    public override void CheckCondition()
    {
        if (!isInit)
        {
            NodeManager.Instance.AddSetTileEvent(HerbTileCheck);
            isInit = true;
        }

        if (curClearNum[0] >= Mathf.Abs(_ClearNum[0]))
            isComplete[0] = true;
    }

    public override void FailQuest()
    {
        base.FailQuest();
        NodeManager.Instance.RemoveSetTileEvent(HerbTileCheck);
    }

    public override void CompleteQuest()
    {
        base.CompleteQuest();
        NodeManager.Instance.RemoveSetTileEvent(HerbTileCheck);
    }
}

public class Quest2003 : Quest
{
    private bool isInit = false;

    private void CheckBuyCardPack(Item item)
    {
        if (item is DebtRepay)
            return;

        curClearNum[0]++;
    }

    public override void CheckCondition()
    {
        if (!isInit)
        {
            GameManager.Instance.shop.AddBuyItemEvent(CheckBuyCardPack);
            isInit = true;
        }

        if (curClearNum[0] >= Mathf.Abs(_ClearNum[0]))
            isComplete[0] = true;
    }

    public override void FailQuest()
    {
        base.FailQuest();
        GameManager.Instance.shop.RemoveBuyItemEvent(CheckBuyCardPack);
    }

    public override void CompleteQuest()
    {
        base.CompleteQuest();
        GameManager.Instance.shop.RemoveBuyItemEvent(CheckBuyCardPack);
    }
}

public class Quest2007 : QuestKillBattler
{
    //마왕 3명처치 이벤트
    protected override void CheckBattlerDead(Battler battler, Battler attecker)
    {
        if (attecker == GameManager.Instance.king)
            curClearNum[0]++;
    }
}

public class Quest2008 : Quest
{
    private TileNode curNode = null;

    public override void CheckCondition()
    {
        if (curNode == null)
            curNode = NodeManager.Instance.endPoint;

        if (curNode != NodeManager.Instance.endPoint)
        {
            isEnd = true;
            base.FailQuest();
        }
    }

    public override void FailQuest()
    {
        isComplete[0] = true;
        CompleteQuest();
    }
}

public class Quest2009 : QuestKillBattler
{
    //영웅도적 처치 퀘스트
    public Quest2009()
    {
        targetId = "s_a40002";
    }
}

public class Quest2010 : Quest
{
    private int curRoomCount = -1;

    public override void CheckCondition()
    {
        if (curRoomCount == -1)
            curRoomCount = NodeManager.Instance.roomTiles.Count;

        if (curRoomCount != NodeManager.Instance.roomTiles.Count)
        {
            curClearNum[0] = NodeManager.Instance.roomTiles[NodeManager.Instance.roomTiles.Count - 1]._IncludeRooms.Count;
            curRoomCount = NodeManager.Instance.roomTiles.Count;
        }

        if (curClearNum[0] >= Mathf.Abs(_ClearNum[0]))
            isComplete[0] = true;
    }

    public override void CompleteQuest()
    {
        base.CompleteQuest();
        //GameManager.Instance.herb1 += 20;
    }

}

public class Quest2011 : Quest
{
    private bool isInit = false;

    private void CheckBuyCardPack(Item item)
    {
        if (item is DebtRepay)
            return;

        curClearNum[0]++;
    }

    public override void CheckCondition()
    {
        if (!isInit)
        {
            GameManager.Instance.shop.AddBuyItemEvent(CheckBuyCardPack);
            isInit = true;
        }

        if (curClearNum[0] >= Mathf.Abs(_ClearNum[0]))
            isComplete[0] = true;
    }

    public override void FailQuest()
    {
        base.FailQuest();
        GameManager.Instance.shop.RemoveBuyItemEvent(CheckBuyCardPack);
    }

    public override void CompleteQuest()
    {
        base.CompleteQuest();
        GameManager.Instance.shop.RemoveBuyItemEvent(CheckBuyCardPack);
    }
}


public class Quest2013 : Quest
{
    //마왕 아우라 30번 부여, 한 몬스터 중복체크X
    private HashSet<Battler> countedMonsters = new HashSet<Battler>();

    private bool isInit = false;

    private void RemoveCountedMonster(Battler battler, Battler attacker)
    {
        if(countedMonsters.Contains(battler))
            countedMonsters.Remove(battler);
    }

    public override void CheckCondition()
    {
        if(!isInit)
        {
            GameManager.Instance.AddBattlerDeadEvent(RemoveCountedMonster);
            isInit = true;
        }

        foreach(var monster in GameManager.Instance.monsterList)
        {
            if (countedMonsters.Contains(monster))
                continue;

            if(monster.HaveEffect<DevilAura>())
            {
                curClearNum[0]++;
                countedMonsters.Add(monster);
            }
        }

        if (curClearNum[0] >= Mathf.Abs(_ClearNum[0]))
            isComplete[0] = true;
    }

    public override void FailQuest()
    {
        base.FailQuest();
        GameManager.Instance.RemoveBattlerDeadEvent(RemoveCountedMonster);
    }

    public override void CompleteQuest()
    {
        base.CompleteQuest();
        GameManager.Instance.RemoveBattlerDeadEvent(RemoveCountedMonster);
    }
}

public class Quest2014 : Quest
{
    //마왕 스킬 사용
    private bool isInit = false;

    private async UniTaskVoid CheckDevilSkill()
    {
        PlayerBattleMain king = GameManager.Instance.king;
        EmergencyEscape escape = new EmergencyEscape();
        ISkill target = king.AddSkill(escape);

        await UniTask.WaitUntil(() => target.coolRate.Value > 0, cancellationToken: king.gameObject.GetCancellationTokenOnDestroy());

        curClearNum[0]++;
        isComplete[0] = true;
    }

    public override void CheckCondition()
    {
        if(!isInit)
        {
            CheckDevilSkill().Forget();
            isInit = true;
        }
    }
}

public class Quest2015 : QuestUniRx
{
    private bool isInit = false;

    //스포너 교체 퀘스트 (철거 후 설치)
    private HashSet<Tile> spawnerTiles = new HashSet<Tile>();

    private bool isCleared = false;

    public override void CheckCondition()
    {
        if(!isInit)
        {
            foreach(var spawner in GameManager.Instance.monsterSpawner)
                spawnerTiles.Add(spawner.curTile);

            GameManager.Instance.monsterSpawner.ObserveAdd().Subscribe(_ =>
            {
                if (spawnerTiles.Contains(_.Value.curTile))
                    isCleared = true;
                else
                    spawnerTiles.Add(_.Value.curTile);
            }).AddTo(cancellationToken.Token);

            isInit = true;
        }

        if (isCleared)
            isComplete[0] = true;
    }
}

public class Quest2016 : Quest
{
    private TileNode curNode = null;
    private int prevTileCount = 0;

    public override void CheckCondition()
    {
        if(curNode != NodeManager.Instance.endPoint || prevTileCount != NodeManager.Instance._ActiveNodes.Count)
        {
            if (NodeManager.Instance.FindPath(NodeManager.Instance.startPoint, NodeManager.Instance.endPoint) != null)
                isComplete[0] = true;
        }

        prevTileCount = NodeManager.Instance._ActiveNodes.Count;
        curNode = NodeManager.Instance.endPoint;
    }
}

public class Quest2018 : QuestUniRx
{
    HashSet<Adventurer> adventurers = new HashSet<Adventurer>();

    private bool isInit = false;

    public override void CheckCondition()
    {
        if(!isInit)
        {
            foreach(var adventurer in GameManager.Instance.adventurersList)
            {
                if (adventurer.BattlerID == "s_boss006")
                {
                    adventurers.Add(adventurer);
                }
            }

            GameManager.Instance.adventurersList.ObserveAdd().Subscribe(_ =>
            {
                if(_.Value.BattlerID == "s_boss006")
                {
                    adventurers.Add(_.Value);
                }
            }).AddTo(cancellationToken.Token);

            GameManager.Instance.adventurersList.ObserveRemove().Subscribe(_ =>
            {
                if (_.Value.BattlerID == "s_boss006")
                {
                    //1. 체력이 0이면 해쉬셋에서 삭제
                    if (_.Value.curHp <= 0)
                    {
                        adventurers.Remove(_.Value);
                        //모두 사망하여 카운트가 0이면 실패
                        if (adventurers.Count == 0)
                            FailQuest();
                    }
                    else
                    {
                        // 체력이 0이 아닌데 삭제되면 귀환임
                        isComplete[0] = true;
                    }
                }
            }).AddTo(cancellationToken.Token);
            isInit = true;
        }
    }
}

public class Quest2019 : Quest
{
    KingSlime king;

    public override void CheckCondition()
    {
        if(king == null)
            king = MonoBehaviour.FindObjectOfType<KingSlime>();

        isComplete[0] = !king.isCollapsed;
    }
}

public class Quest2020 : Quest
{

    private int curPathCount = -1;

    public override void CheckCondition()
    {
        if (curPathCount == -1)
            curPathCount = NodeManager.Instance.tileDictionary[TileType.Path].Count;

        //길이 새로 설치될 때 길의 연속개수 확인
        if (NodeManager.Instance.tileDictionary[TileType.Path].Count > curPathCount)
        {
            Tile lastTile = NodeManager.Instance.tileDictionary[TileType.Path][NodeManager.Instance.tileDictionary[TileType.Path].Count - 1];
            int curNumber = UtilHelper.GetPathCount(lastTile).Count;
            if (curNumber > curClearNum[0])
                curClearNum[0] = curNumber;
        }

        curPathCount = NodeManager.Instance.tileDictionary[TileType.Path].Count;
        if (curClearNum[0] >= Mathf.Abs(_ClearNum[0]))
            isComplete[0] = true;
    }
}

public class Quest2021 : Quest
{
    private bool isInit = false;
    private QuestDebtRepay debtQuest;
    private int prevCount;

    public override void CheckCondition()
    {
        if(!isInit)
        {
            if(QuestManager.Instance.questController.mainQuest is  QuestDebtRepay quest)
            {
                debtQuest = quest;
            }

            prevCount = debtQuest._CurClearNum[0];
            isInit = true;
        }

        if(prevCount < debtQuest._CurClearNum[0])
            curClearNum[0]++;
        prevCount = debtQuest._CurClearNum[0];

        if (curClearNum[0] >= Mathf.Abs(_ClearNum[0]))
            isComplete[0] = true;
    }
}

public class Quest3101 : SetTileQuest
{
    //길 타일 설치 퀘스트 + 카드 뽑기
    private int cardCount = -1;

    protected override void InitEvent()
    {
        base.InitEvent();
        cardCount = GameManager.Instance.cardDeckController.hand_CardNumber;
    }

    protected override bool SetTileCheck(ITileKind tileKind)
    {
        if (tileKind is Tile tile && tile._TileType == TileType.Path)
        {
            curClearNum[0]++;
            return true;
        }

        return false;
    }

    public override void CheckCondition()
    {
        if (!isComplete[1] && GameManager.Instance.cardDeckController.hand_CardNumber > cardCount)
        {
            curClearNum[1] += GameManager.Instance.cardDeckController.hand_CardNumber - cardCount;
        }
        cardCount = GameManager.Instance.cardDeckController.hand_CardNumber;

        for (int i = 0; i < curClearNum.Count; i++)
        {
            if (curClearNum[i] >= Mathf.Abs(_ClearNum[i]))
                isComplete[i] = true;
        }
    }
}

public class Quest3102 : Quest
{
    private TileNode curNode = null;

    public override void CheckCondition()
    {
        if (curNode == null)
            curNode = NodeManager.Instance.endPoint;

        if (curNode != NodeManager.Instance.endPoint)
        {
            curClearNum[0]++;
            isComplete[0] = true;
        }
    }
}

public class Quest3103 : QuestUniRx
{
    protected override void InitEvent()
    {
        GameManager.Instance.trapList.ObserveAdd().Subscribe(_ => curClearNum[0]++).AddTo(cancellationToken.Token);
    }

    public override void CheckCondition()
    {
        if (curClearNum[0] >= Mathf.Abs(_ClearNum[0]))
            isComplete[0] = true;
    }
}

public class Quest3104 : SetTileQuest
{
    protected override bool SetTileCheck(ITileKind tileKind)
    {
        if (tileKind is Tile tile && tile._TileType == TileType.Room_Single)
        {
            curClearNum[0]++;
            return true;
        }

        return false;
    }

    public override void CheckCondition()
    {
        if (curClearNum[0] >= Mathf.Abs(_ClearNum[0]))
            isComplete[0] = true;
    }
}

public class Quest3105 : QuestUniRx
{
    //슬라임 스포너 3개 설치 퀘스트
    protected override void InitEvent()
    {
        GameManager.Instance.monsterSpawner.ObserveAdd().Where(_ => _.Value._TargetName.Contains("slime")).Subscribe(_ => curClearNum[0]++).AddTo(cancellationToken.Token);
    }

    public override void CheckCondition()
    {
        if (curClearNum[0] >= Mathf.Abs(_ClearNum[0]))
            isComplete[0] = true;
    }
}

public class Quest3106 : SetTileQuest
{
    protected override bool SetTileCheck(ITileKind tileKind)
    {
        if (tileKind is Environment)
        {
            curClearNum[0]++;
            return true;
        }

        return false;
    }

    public override void CheckCondition()
    {
        if (curClearNum[0] >= Mathf.Abs(_ClearNum[0]))
            isComplete[0] = true;
    }
}

public class Quest3107 : Quest
{
    private int curHiddenCount = -1;
    protected override void InitEvent() => curHiddenCount = NodeManager.Instance.hiddenTiles.Count;

    public override void CheckCondition()
    {
        if (NodeManager.Instance.hiddenTiles.Count < curHiddenCount)
            curClearNum[0]++;

        curHiddenCount = NodeManager.Instance.hiddenTiles.Count;

        if (curClearNum[0] >= Mathf.Abs(_ClearNum[0]))
            isComplete[0] = true;
    }
}

public class Quest3108 : ShopBuyQuest
{
    protected override void CheckBuy(Item item)
    {
        if (item is DebtRepay)
            return;

        curClearNum[0]++;
    }

    public override void CheckCondition()
    {
        if (curClearNum[0] >= Mathf.Abs(_ClearNum[0]))
            isComplete[0] = true;
    }
}

public class Quest3109 : Quest
{
    float prevTimeScale = -1;
    protected override void InitEvent() => prevTimeScale = GameManager.Instance.timeScale;

    public override void CheckCondition()
    {
        if (prevTimeScale != GameManager.Instance.timeScale && UIManager.Instance._OpendUICount == 0 && !GameManager.Instance.isPause)
        {
            curClearNum[0]++;
            isComplete[0] = true;
        }
    }
}

public class Quest3110 : Quest
{
    public override void CheckCondition()
    {
        if (GameManager.Instance.research.IsResearchCompleted("r_m30001"))
        {
            curClearNum[0]++;
            isComplete[0] = true;
        }
    }
}

public class Quest3111 : RemoveTileQuest
{
    protected override bool RemoveTileCheck(ITileKind tileKind)
    {
        curClearNum[0]++;

        return true;
    }

    public override void CheckCondition()
    {
        if (curClearNum[0] >= Mathf.Abs(_ClearNum[0]))
            isComplete[0] = true;
    }

    public override void FailQuest()
    {
        base.FailQuest();
        GameManager.Instance.gold = Mathf.Max(GameManager.Instance.gold - 300, 0);
    }
}

public class Questq3112 : QuestKillBattler
{
    //마을이장 처치 퀘스트
    public Questq3112()
    {
        targetId = "s_boss007";
    }
}

public class Quest3113 : Quest
{
    public override void CheckCondition()
    {
        if (GameManager.Instance.research.IsResearchCompleted("r_t10002"))
        {
            curClearNum[0]++;
            isComplete[0] = true;
        }
    }
}

public class Quest3114 : Quest
{
    private int curRoomCount = -1;

    protected override void InitEvent() => curRoomCount = NodeManager.Instance.roomTiles.Count;

    public override void CheckCondition()
    {
        if (curRoomCount != NodeManager.Instance.roomTiles.Count)
        {
            curClearNum[0] = NodeManager.Instance.roomTiles[NodeManager.Instance.roomTiles.Count - 1]._IncludeRooms.Count;
            curRoomCount = NodeManager.Instance.roomTiles.Count;
        }

        if (curClearNum[0] >= Mathf.Abs(_ClearNum[0]))
            isComplete[0] = true;
    }
}

public class Quest3115 : Quest
{
    public bool isFind = false;
    public override void CheckCondition()
    {
        if (isFind)
            isComplete[0] = true;
    }
}

public class Quest3116 : QuestKillBattler
{
    //도적 처치 퀘스트
    public Quest3116()
    {
        targetId = "s_a40001";
    }
}

public class Quest3117 : Quest
{
    public bool isFind = false;
    public override void CheckCondition()
    {
        if (isFind)
            isComplete[0] = true;
    }
}

public class Quest3118 : QuestUniRx
{
    //골렘 스포너 설치 퀘스트
    protected override void InitEvent()
    {
        GameManager.Instance.monsterSpawner.ObserveAdd().Where(_ => _.Value._TargetName.Contains("golem")).Subscribe(_ => curClearNum[0]++).AddTo(cancellationToken.Token);
    }

    public override void CheckCondition()
    {
        if (curClearNum[0] >= Mathf.Abs(_ClearNum[0]))
            isComplete[0] = true;
    }
}

public class Quest3119 : QuestKillBattler
{
    //길드장 처치 퀘스트
    public Quest3119()
    {
        targetId = "s_boss002";
    }

    //임시로 승리
    public override void CompleteQuest()
    {
        base.CompleteQuest();
        GameManager.Instance.WinGame();
    }
}