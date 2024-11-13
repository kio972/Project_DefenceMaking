using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quest0001 : Quest
{
    public override void CheckCondition()
    {
        curClearNum[0] = _ClearNum[0] - Mathf.FloorToInt(_CurTime / 1440);
    }

    public override void CompleteQuest()
    {
        base.CompleteQuest();
        GameManager.Instance.WinGame();
        SaveManager.Instance.settingData.stageState = 1;
        SaveManager.Instance.SaveSettingData();
    }

    public override void FailQuest()
    {
        CompleteQuest();
    }

    public override void UpdateQuest()
    {
        if (_CurTime > _TimeLimit)
            isComplete[0] = true;
        base.UpdateQuest();
    }
}

public class Quest0101 : Quest
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

public class Quest0102 : Quest
{
    private bool isInit;
    private int monsterCount = -1;
    private int spawnerCount = -1;

    private bool IncreaseCount(ITileKind tileKind)
    {
        if (tileKind is TileHidden || tileKind.curNode == NodeManager.Instance.endPoint)
            return false;
        if (tileKind is Tile tile && tile.IsDormant)
            return false;

        curClearNum[0]++;
        return true;
    }

    public override void CheckCondition()
    {
        if (!isInit)
        {
            NodeManager.Instance.AddSetTileEvent(IncreaseCount);
            monsterCount = GameManager.Instance.monsterList.Count;
            spawnerCount = GameManager.Instance.monsterSpawner.Count;
            isInit = true;
        }

        if (!isComplete[0] && curClearNum[0] >= Mathf.Abs(_ClearNum[0]))
            isComplete[0] = true;

        if (!isComplete[1] && GameManager.Instance.monsterList.Count > monsterCount)
        {
            curClearNum[1]++;
            isComplete[1] = true;
        }

        if (!isComplete[2] && GameManager.Instance.monsterSpawner.Count > spawnerCount)
        {
            curClearNum[2]++;
            isComplete[2] = true;
        }

        monsterCount = GameManager.Instance.monsterList.Count;
        spawnerCount = GameManager.Instance.monsterSpawner.Count;
    }

    public override void FailQuest()
    {
        base.FailQuest();
        NodeManager.Instance.RemoveSetTileEvent(IncreaseCount);
    }

    public override void CompleteQuest()
    {
        base.CompleteQuest();
        NodeManager.Instance.RemoveSetTileEvent(IncreaseCount);
    }
}

public class Quest0103 : Quest
{

    float prevTimeScale = -1;

    public override void CheckCondition()
    {
        if (prevTimeScale == -1)
            prevTimeScale = GameManager.Instance.timeScale;

        if (prevTimeScale != GameManager.Instance.timeScale && UIManager.Instance._OpendUICount == 0 && !GameManager.Instance.isPause)
        {
            curClearNum[0]++;
            isComplete[0] = true;
        }
    }
}

public class Quest0104 : Quest
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
}

public class Quest0105 : Quest
{
    private int cardCount = -1;

    public override void CheckCondition()
    {
        if (cardCount == -1)
        {
            cardCount = GameManager.Instance.cardDeckController.hand_CardNumber;
        }

        if (!isComplete[0] && GameManager.Instance.cardDeckController.hand_CardNumber > cardCount)
        {
            curClearNum[0]++;
        }

        if (curClearNum[0] >= Mathf.Abs(_ClearNum[0]))
        {
            isComplete[0] = true;
        }

        cardCount = GameManager.Instance.cardDeckController.hand_CardNumber;
    }
}

public class Quest0106 : Quest
{
    //Ä«µåÆÑ ±¸¸Å Äù½ºÆ®
    private bool isInit = false;

    private void CheckBuyCardPack(Item item)
    {
        if (item is not CardPack)
            return;

        curClearNum[0]++;
        isComplete[0] = true;
    }

    public override void CheckCondition()
    {
        if (!isInit)
        {
            GameManager.Instance.shop.AddBuyItemEvent(CheckBuyCardPack);
            isInit = true;
        }
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