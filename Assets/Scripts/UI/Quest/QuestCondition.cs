using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class QuestCondition
{
    public abstract bool IsConditionPassed();
}

public class QusetCondition_m0105 : QuestCondition
{
    public override bool IsConditionPassed()
    {
        return GameManager.Instance.cardDeckController.hand_CardNumber == 0;
    }
}

public class QusetCondition_m0106 : QuestCondition
{
    public override bool IsConditionPassed()
    {
        return GameManager.Instance.cardDeckController.cardDeckCount == 0;
    }
}

public class QusetCondition_m2001 : QuestCondition
{
    public override bool IsConditionPassed()
    {
        return QuestManager.Instance.questController.mainQuest._QuestID == "q1001";
    }
}

public class QusetCondition_m2003 : QuestCondition
{
    public override bool IsConditionPassed()
    {
        return QuestManager.Instance.questController.subQuest.Where(_ => _._QuestID == "q2002").Count() >= 1;
    }
}

//public class QusetCondition_m2007 : QuestCondition
//{
//    public override bool IsConditionPassed()
//    {
//        return QuestManager.Instance.questController._SubQuest.Where(_ => _._QuestID == "q2006").Count() >= 1;
//    }
//}

public class QusetCondition_m2012 : QuestCondition
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

public class QusetCondition_m2013 : QuestCondition
{
    public override bool IsConditionPassed()
    {
        return QuestManager.Instance.questController.subQuest.Where(_ => _._QuestID == "q2004").Count() >= 1;
    }
}