using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Quest2001 : Quest
{
    private int startMonsterCount = -1;
    private int startTrapCount = -1;

    public override void CheckCondition()
    {
        if (startMonsterCount == -1)
            startMonsterCount = GameManager.Instance.monsterSpawner.Count;
        if (startTrapCount == -1)
            startTrapCount = GameManager.Instance.trapList.Count;

        if(GameManager.Instance.monsterSpawner.Count > startMonsterCount || GameManager.Instance.trapList.Count > startTrapCount)
            isComplete[0] = true;
    }

    public override void CompleteQuest()
    {
        base.CompleteQuest();
        GameManager.Instance.gold += 100;
    }
}

public class Quest2002 : Quest
{
    private bool isInit;

    private bool IncreaseCount(GameObject tile)
    {
        if (tile.GetComponent<TileHidden>() != null)
            return false;

        if (tile.GetComponent<Tile>() != null && tile.GetComponent<Tile>().curNode == NodeManager.Instance.endPoint)
            return false;

        if (tile.GetComponent<Tile>() != null && tile.GetComponent<Tile>().IsDormant)
            return false;

        curClearNum[0]++;
        return true;
    }

    public override void CheckCondition()
    {
        if (!isInit)
        {
            NodeManager.Instance.AddSetTileEvent(IncreaseCount);
            isInit = true;
        }

        if (curClearNum[0] >= Mathf.Abs(_ClearNum[0]))
            isComplete[0] = true;
    }

    public override void FailQuest()
    {
        base.FailQuest();
        NodeManager.Instance.RemoveSetTileEvent(IncreaseCount);
    }

    public override void CompleteQuest()
    {
        base.CompleteQuest();
        GameManager.Instance.gold += 100;
        NodeManager.Instance.RemoveSetTileEvent(IncreaseCount);
    }
}

public class Quest2003 : Quest
{
    private TileNode curNode = null;

    public override void CheckCondition()
    {
        if (curNode == null)
            curNode = NodeManager.Instance.endPoint;

        if (curNode != NodeManager.Instance.endPoint)
            isComplete[0] = true;
    }

    public override void CompleteQuest()
    {
        base.CompleteQuest();
        GameManager.Instance.herb1 += 5;
    }
}

//public class Quest2004 : Quest
//{
//    private int curRoomCount = -1;

//    public override void CheckCondition()
//    {
//        if (curRoomCount == -1)
//            curRoomCount = NodeManager.Instance.tileDictionary[TileType.Room].Count + NodeManager.Instance.tileDictionary[TileType.Room_Single].Count + NodeManager.Instance.tileDictionary[TileType.Door].Count;
//        curClearNum[0] += NodeManager.Instance.tileDictionary[TileType.Room].Count + NodeManager.Instance.tileDictionary[TileType.Room_Single].Count + NodeManager.Instance.tileDictionary[TileType.Door].Count - curRoomCount;
//        curRoomCount = NodeManager.Instance.tileDictionary[TileType.Room].Count + NodeManager.Instance.tileDictionary[TileType.Room_Single].Count + NodeManager.Instance.tileDictionary[TileType.Door].Count;
//        if (curClearNum[0] >= Mathf.Abs(_ClearNum[0]))
//            isComplete[0] = true;
//    }

//    public override void CompleteQuest()
//    {
//        base.CompleteQuest();
//        GameManager.Instance.gold += 100;
//    }
//}

public class Quest2005 : Quest
{
    private int curResearchCount = -1;

    ResearchMainUI researchMain = null;

    public override void CheckCondition()
    {
        if (curResearchCount == -1)
            curResearchCount = GameManager.Instance.research.completedResearchs.Count;

        if(GameManager.Instance.research.completedResearchs.Count > curResearchCount)
            isComplete[0] = true;
    }

    public override void CompleteQuest()
    {
        base.CompleteQuest();
        GameManager.Instance.gold += 200;
    }
}

//public class Quest2006 : Quest
//{
//    private int curEnvironmentCount = -1;

//    public override void CheckCondition()
//    {
//        if (curEnvironmentCount == -1)
//            curEnvironmentCount = NodeManager.Instance.environments.Count;
//        curClearNum[0] += NodeManager.Instance.environments.Count - curEnvironmentCount;
//        curEnvironmentCount = NodeManager.Instance.environments.Count;

//        if (curClearNum[0] >= Mathf.Abs(_ClearNum[0]))
//            isComplete[0] = true;
//    }

//    public override void CompleteQuest()
//    {
//        base.CompleteQuest();
//        GameManager.Instance.gold += 300;
//    }
//}

public class Quest2007 : Quest
{
    private ItemSlot pathPack = null;
    public override void CheckCondition()
    {
        if (pathPack == null)
        {
            CardPack[] packes = GameManager.Instance.shop.GetComponentsInChildren<CardPack>(true);
            foreach (CardPack pack in packes)
            {
                ItemSlot slot = pack.GetComponent<ItemSlot>();
                if (slot.ItemId == "shop_item0004")
                {
                    pathPack = slot;
                    break;
                }
            }
        }

        if (pathPack.IsSoldOut)
            isComplete[0] = true;
    }

    public override void CompleteQuest()
    {
        base.CompleteQuest();
        GameManager.Instance.gold += 100;
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
        CompleteQuest();
    }

    public override void CompleteQuest()
    {
        base.CompleteQuest();
        GameManager.Instance.gold += 1000;
    }
}

public class Quest2009 : Quest
{
    private Adventurer bossAdventurer = null;

    public override void CheckCondition()
    {
        if (bossAdventurer == null && GameManager.Instance.adventurersList.Count != 0)
        {
            Adventurer target = GameManager.Instance.adventurersList[GameManager.Instance.adventurersList.Count - 1];
            if (target.BattlerID == "s_boss001")
                bossAdventurer = target;
        }

        if (bossAdventurer == null)
            return;

        if (bossAdventurer.isDead)
            isComplete[0] = true;
    }

    public override void CompleteQuest()
    {
        base.CompleteQuest();
        GameManager.Instance.herb3 += 20;
    }
}

public class Quest2010 : Quest
{
    private int curPathCount = -1;

    private int GetPathCount(Tile startTile)
    {
        List<Tile> visited = new List<Tile> { startTile };
        Queue<Tile> queue = new Queue<Tile>();
        queue.Enqueue(startTile);

        while (queue.Count != 0)
        {
            Tile next = queue.Dequeue();
            foreach (Direction dir in next.PathDirection)
            {
                TileNode targetNode = next.curNode.DirectionalNode(dir);
                // 대상 방향에 길 타일 없음
                if (targetNode == null || targetNode.curTile == null || targetNode.curTile._TileType != TileType.Path)
                    continue;

                // 이미 방문한 타일임
                if (visited.Contains(targetNode.curTile))
                    continue;

                // 대상 방향으로 길 연결되어있지 않음
                if (!targetNode.curTile.PathDirection.Contains(UtilHelper.ReverseDirection(dir)))
                    continue;

                queue.Enqueue(targetNode.curTile);
                visited.Add(targetNode.curTile);
            }
        }

        return visited.Count;
    }

    public override void CheckCondition()
    {
        if (curPathCount == -1)
            curPathCount = NodeManager.Instance.tileDictionary[TileType.Path].Count;

        //길이 새로 설치될 때 길의 연속개수 확인
        if (NodeManager.Instance.tileDictionary[TileType.Path].Count > curPathCount)
        {
            Tile lastTile = NodeManager.Instance.tileDictionary[TileType.Path][NodeManager.Instance.tileDictionary[TileType.Path].Count - 1];
            int curNumber = GetPathCount(lastTile);
            if (curNumber > curClearNum[0])
                curClearNum[0] = curNumber;
        }

        curPathCount = NodeManager.Instance.tileDictionary[TileType.Path].Count;
        if (curClearNum[0] >= Mathf.Abs(_ClearNum[0]))
            isComplete[0] = true;
    }

    public override void CompleteQuest()
    {
        base.CompleteQuest();
        GameManager.Instance.herb1 += 20;
    }
}

public class Quest2011 : Quest
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
        GameManager.Instance.herb1 += 20;
    }
}

public class Quest2012 : Quest
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

    public override void CompleteQuest()
    {
        base.CompleteQuest();
        GameManager.Instance.herb2 += 30;
    }
}

public class Quest2013 : Quest
{
    GameObject guide = null;

    public override void CheckCondition()
    {
        if (guide == null)
        {
            bool isPause = GameManager.Instance.isPause;
            SettingCanvas.Instance.CallSettings(false);
            GameManager.Instance.isPause = isPause;

            guide = SettingCanvas.Instance.transform.GetComponentInChildren<GuideSpiner>(true).transform.parent.gameObject;
        }
        
        if (guide.activeSelf)
            isComplete[0] = true;
    }

    public override void CompleteQuest()
    {
        base.CompleteQuest();
        GameManager.Instance.herb1 += 10;
    }
}


public class Quest2014 : Quest
{
    public override void CheckCondition()
    {
        isComplete[0] = true;
    }

    public override void CompleteQuest()
    {
        base.CompleteQuest();
        GameManager.Instance.herb1 += 10;
        GameManager.Instance.herb2 += 10;
        GameManager.Instance.herb3 += 10;
    }
}