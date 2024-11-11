using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using UniRx;

public abstract class QuestCondition
{
    public abstract bool IsConditionPassed();
}

public class QuestCondition_m0105 : QuestCondition
{
    public override bool IsConditionPassed()
    {
        return GameManager.Instance.cardDeckController.hand_CardNumber == 0;
    }
}

public class QuestCondition_m0106 : QuestCondition
{
    public override bool IsConditionPassed()
    {
        return GameManager.Instance.cardDeckController.cardDeckCount == 0;
    }
}

public class QuestCondition_m2002 : QuestCondition
{
    public override bool IsConditionPassed()
    {
        foreach (var index in GameManager.Instance.cardDeckController.handCards)
        {
            if(DataManager.Instance.herbCard_Indexs.Contains(index))
                return true;
        }
        return false;
    }
}

public class QuestCondition_m2015 : QuestCondition
{
    public override bool IsConditionPassed()
    {
        return PassiveManager.Instance.deployAvailableTable.Count >= 2;
    }
}

public class QuestCondition_m2016 : QuestCondition
{
    bool isConnectionLost = false;

    private TileNode prevNode = null;
    private int activeCount;

    public override bool IsConditionPassed()
    {
        if(prevNode != NodeManager.Instance.endPoint || activeCount > NodeManager.Instance._ActiveNodes.Count) //마왕타일이 옮겨지거나 타일이 삭제될경우 갱신
            isConnectionLost = PathFinder.FindPath(NodeManager.Instance.startPoint, NodeManager.Instance.endPoint) == null;

        prevNode = NodeManager.Instance.endPoint;
        activeCount = NodeManager.Instance._ActiveNodes.Count;

        return isConnectionLost;
    }
}

public class QuestCondition_m2017 : QuestCondition
{
    private int prevHiddenTileCount = -1;

    public override bool IsConditionPassed()
    {
        if (prevHiddenTileCount == -1)
            prevHiddenTileCount = NodeManager.Instance.hiddenTiles.Count;

        if(NodeManager.Instance.hiddenTiles.Count < prevHiddenTileCount)
            return true;
        else
        {
            prevHiddenTileCount = NodeManager.Instance.hiddenTiles.Count;
            return false;
        }
    }
}

public class QuestCondition_m2018 : QuestCondition
{
    public override bool IsConditionPassed()
    {
        if (GameManager.Instance.CurWave <= 9)
            return false;
        else if (GameManager.Instance.CurWave <= 19)
            return QuestManager.Instance.IsQuestFailed("q1001");
        else if (GameManager.Instance.CurWave <= 29)
            return QuestManager.Instance.IsQuestFailed("q1002");
        else if (GameManager.Instance.CurWave <= 39)
            return QuestManager.Instance.IsQuestFailed("q1003");
        else if (GameManager.Instance.CurWave <= 49)
            return QuestManager.Instance.IsQuestFailed("q1004");
        else if (GameManager.Instance.CurWave <= 59)
            return QuestManager.Instance.IsQuestFailed("q1005");

        return false;
    }
}

public class QuestCondition_m2019 : QuestCondition
{
    private CancellationTokenSource cancellToken = new CancellationTokenSource();
    private bool isPassed = false;
    private void CheckSlimeKing(Monster monster)
    {
        if (monster.BattlerID == "s_m10004")
            isPassed = true;
    }

    public override bool IsConditionPassed()
    {
        GameManager.Instance.monsterList.ObserveAdd().Subscribe(_ => CheckSlimeKing(_.Value)).AddTo(cancellToken.Token);

        if(isPassed)
        {
            cancellToken.Cancel();
            cancellToken.Dispose();
        }

        return isPassed;
    }
}

public class QuestCondition_m2020 : QuestCondition
{
    public override bool IsConditionPassed() => false;
}

//public class QuestCondition_m2013 : QuestCondition
//{
//    public override bool IsConditionPassed()
//    {
//        return QuestManager.Instance.questController.subQuest.Where(_ => _._QuestID == "q2004").Count() >= 1;
//    }
//}