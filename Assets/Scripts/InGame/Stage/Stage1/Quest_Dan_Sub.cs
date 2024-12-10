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
    //카드팩 구매 퀘스트
    private bool isInit = false;

    private void CheckBuyCardPack(Item item)
    {
        if (item is CardPack)
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

public class Quest2004 : QuestKillBattler
{
    //도적 처치 퀘스트
    public Quest2004()
    {
        targetId = "s_a40001";
    }
}

public class Quest2005 : Quest
{
    //환경타일 카드팩 구매 퀘스트
    private bool isInit = false;

    private void CheckBuyCardPack(Item item)
    {
        if (item is CardPack pack)
        {
            if(pack.cardType == CardPackType.Environment)
            {
                curClearNum[0]++;
            }
        }
    }

    public override void CheckCondition()
    {
        if (!isInit)
            GameManager.Instance.shop.AddBuyItemEvent(CheckBuyCardPack);

        if (curClearNum[0] >= Mathf.Abs(_ClearNum[0]))
            isComplete[0] = true;
    }


    public override void FailQuest()
    {
        base.FailQuest();
        GameManager.Instance.shop.AddBuyItemEvent(CheckBuyCardPack);
    }

    public override void CompleteQuest()
    {
        base.CompleteQuest();
        GameManager.Instance.shop.AddBuyItemEvent(CheckBuyCardPack);
    }
}

public class Quest2006 : QuestKillBattler
{
    //마을이장 처치 퀘스트
    public Quest2006()
    {
        targetId = "s_boss007";
    }

    public override void CompleteQuest()
    {
        base.CompleteQuest();
        GameManager.Instance.WinGame();
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
    //카드팩 구매 퀘스트
    private bool isInit = false;

    private void CheckBuyCardPack(Item item)
    {
        if (item is CardPack)
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

public class Quest2012 : Quest
{
    //카드팩 구매 퀘스트
    private bool isInit = false;

    public bool RemoveTileEvent(ITileKind tileKind)
    {
        curClearNum[0]++;

        return true;
    }


    public override void CheckCondition()
    {
        if (!isInit)
        {
            NodeManager.Instance.AddRemoveTileEvent(RemoveTileEvent);
            isInit = true;
        }

        if (curClearNum[0] >= Mathf.Abs(_ClearNum[0]))
            isComplete[0] = true;
    }

    public override void FailQuest()
    {
        base.FailQuest();
        NodeManager.Instance.RemoveRemoveTileEvent(RemoveTileEvent);
        GameManager.Instance.gold = Mathf.Max(GameManager.Instance.gold - 300, 0);
    }

    public override void CompleteQuest()
    {
        base.CompleteQuest();
        NodeManager.Instance.RemoveRemoveTileEvent(RemoveTileEvent);
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
        king.skills.Add(escape);
        escape.SkillInit();

        //await UniTask.WaitUntil(() => king.skills.Count > 0, cancellationToken: king.gameObject.GetCancellationTokenOnDestroy());

        //await UniTask.Yield(cancellationToken: king.gameObject.GetCancellationTokenOnDestroy());

        await UniTask.WaitUntil(() => escape.coolRate.Value > 0, cancellationToken: king.gameObject.GetCancellationTokenOnDestroy());

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

public class Quest2015 : Quest
{
    private bool isInit = false;

    //스포너 교체 퀘스트 (철거 후 설치)
    private HashSet<Tile> spawnerTiles = new HashSet<Tile>();

    private bool isCleared = false;
    private CancellationTokenSource cancellationToken = new CancellationTokenSource();

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

public class Quest2017 : Quest
{
    private int curHiddenCount = -1;

    public override void CheckCondition()
    {
        if (curHiddenCount == -1)
            curHiddenCount = NodeManager.Instance.hiddenTiles.Count;

        if (NodeManager.Instance.hiddenTiles.Count < curHiddenCount)
            curClearNum[0]++;

        curHiddenCount = NodeManager.Instance.hiddenTiles.Count;

        if (curClearNum[0] >= Mathf.Abs(_ClearNum[0]))
            isComplete[0] = true;
    }
}

public class Quest2018 : Quest
{
    CancellationTokenSource cancellationToken = new CancellationTokenSource();
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
    private ItemSlot debtRepayItem;
    private int prevCount;

    public override void CheckCondition()
    {
        if(!isInit)
        {
            foreach(var item in GameManager.Instance.shop.itemSlots)
            {
                if(item.item is DebtRepay repay && repay.IsUnlock)
                {
                    debtRepayItem = item;
                    break;
                }
            }
            prevCount = debtRepayItem.curStockCount;
            isInit = true;
        }

        if(debtRepayItem.curStockCount < prevCount)
            curClearNum[0] += prevCount - debtRepayItem.curStockCount;
        prevCount = debtRepayItem.curStockCount;

        if (curClearNum[0] >= Mathf.Abs(_ClearNum[0]))
            isComplete[0] = true;
    }
}