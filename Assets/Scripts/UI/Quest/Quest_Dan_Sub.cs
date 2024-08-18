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
    private int curPathCount = -1;

    public override void CheckCondition()
    {
        if (curPathCount == -1)
            curPathCount = NodeManager.Instance.tileDictionary[TileType.Path].Count;
        curClearNum[0] += (NodeManager.Instance.tileDictionary[TileType.Path].Count - curPathCount);
        curPathCount = NodeManager.Instance.tileDictionary[TileType.Path].Count;
        if (curClearNum[0] >= Mathf.Abs(_ClearNum[0]))
            isComplete[0] = true;
    }

    public override void CompleteQuest()
    {
        base.CompleteQuest();
        GameManager.Instance.gold += 100;
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
            curClearNum[0]++;

        curNode = NodeManager.Instance.endPoint;

        if (curClearNum[0] >= Mathf.Abs(_ClearNum[0]))
            isComplete[0] = true;
    }

    public override void CompleteQuest()
    {
        base.CompleteQuest();
        GameManager.Instance.herb1 += 5;
    }
}

public class Quest2004 : Quest
{
    private int curRoomCount = -1;

    public override void CheckCondition()
    {
        if (curRoomCount == -1)
            curRoomCount = NodeManager.Instance.tileDictionary[TileType.Room].Count + NodeManager.Instance.tileDictionary[TileType.Room_Single].Count + NodeManager.Instance.tileDictionary[TileType.Door].Count;
        curClearNum[0] += NodeManager.Instance.tileDictionary[TileType.Room].Count + NodeManager.Instance.tileDictionary[TileType.Room_Single].Count + NodeManager.Instance.tileDictionary[TileType.Door].Count - curRoomCount;
        curRoomCount = NodeManager.Instance.tileDictionary[TileType.Room].Count + NodeManager.Instance.tileDictionary[TileType.Room_Single].Count + NodeManager.Instance.tileDictionary[TileType.Door].Count;
        if (curClearNum[0] >= Mathf.Abs(_ClearNum[0]))
            isComplete[0] = true;
    }

    public override void CompleteQuest()
    {
        base.CompleteQuest();
        GameManager.Instance.gold += 100;
    }
}

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

public class Quest2006 : Quest
{
    private int curEnvironmentCount = -1;

    public override void CheckCondition()
    {
        if (curEnvironmentCount == -1)
            curEnvironmentCount = NodeManager.Instance.environments.Count;
        curClearNum[0] += NodeManager.Instance.environments.Count - curEnvironmentCount;
        curEnvironmentCount = NodeManager.Instance.environments.Count;

        if (curClearNum[0] >= Mathf.Abs(_ClearNum[0]))
            isComplete[0] = true;
    }

    public override void CompleteQuest()
    {
        base.CompleteQuest();
        GameManager.Instance.gold += 300;
    }
}

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
    public override void CheckCondition()
    {
        //로직 작성 필요
        //길타일을 새로 설치할 때, 해당 타일에서 탐색돌려서 길이가 5인지 확인
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