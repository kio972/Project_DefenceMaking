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

    public override void CompleteQuest()
    {
        base.CompleteQuest();
        GameManager.Instance.gold += 10;
    }
}

public class Quest0102 : Quest
{
    private bool isInit;
    private int monsterCount = -1;
    private int spawnerCount = -1;

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
            monsterCount = GameManager.Instance._MonsterList.Count;
            spawnerCount = GameManager.Instance.monsterSpawner.Count;
            isInit = true;
        }

        if (!isComplete[0] && curClearNum[0] >= Mathf.Abs(_ClearNum[0]))
            isComplete[0] = true;

        if (!isComplete[1] && GameManager.Instance._MonsterList.Count > monsterCount)
        {
            curClearNum[1]++;
            isComplete[1] = true;
        }

        if (!isComplete[2] && GameManager.Instance.monsterSpawner.Count > spawnerCount)
        {
            curClearNum[2]++;
            isComplete[2] = true;
        }

        monsterCount = GameManager.Instance._MonsterList.Count;
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
        GameManager.Instance.gold += 10;
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

    public override void CompleteQuest()
    {
        base.CompleteQuest();
        GameManager.Instance.gold += 10;
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

    public override void CompleteQuest()
    {
        base.CompleteQuest();
        GameManager.Instance.gold += 10;
    }
}

public class Quest0105 : Quest
{
    private int cardCount = -1;
    private int deckCount = -1;

    public override void CheckCondition()
    {
        if (cardCount == -1 || deckCount == -1)
        {
            cardCount = GameManager.Instance.cardDeckController.hand_CardNumber;
            deckCount = GameManager.Instance.cardDeckController.cardDeckCount;
        }

        if (!isComplete[0] && GameManager.Instance.cardDeckController.hand_CardNumber > cardCount)
        {
            curClearNum[0]++;
            isComplete[0] = true;
        }

        if (!isComplete[1] && GameManager.Instance.cardDeckController.hand_CardNumber < cardCount && GameManager.Instance.cardDeckController.cardDeckCount > deckCount)
        {
            curClearNum[1]++;
            isComplete[1] = true;
        }

        cardCount = GameManager.Instance.cardDeckController.hand_CardNumber;
        deckCount = GameManager.Instance.cardDeckController.cardDeckCount;
    }

    public override void CompleteQuest()
    {
        base.CompleteQuest();
        GameManager.Instance.gold += 10;
    }
}

public class Quest0106 : Quest
{
    private bool[] shopList = null;
    private ItemSlot[] packs;

    private bool IsItemSold
    {
        get
        {
            for (int i = 0; i < shopList.Length; i++)
            {
                if (shopList[i] != packs[i].IsSoldOut && packs[i].IsSoldOut)
                    return true;
            }

            return false;
        }
    }

    public override void CheckCondition()
    {
        if (shopList == null)
        {
            packs = GameManager.Instance.shop.GetComponentsInChildren<ItemSlot>(true);
            shopList = new bool[packs.Length];
            for (int i = 0; i < packs.Length; i++)
                shopList[i] = packs[i].IsSoldOut;
        }

        if (IsItemSold)
        {
            curClearNum[0]++;
            isComplete[0] = true;
        }
    }

    public override void CompleteQuest()
    {
        base.CompleteQuest();
        GameManager.Instance.gold += 10;
    }
}

public class Quest0107 : Quest
{
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

    public override void CompleteQuest()
    {
        base.CompleteQuest();
        GameManager.Instance.gold += 10;
    }
}