using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Quest
{
    private string questID;
    public string _QuestID { get => questID; }
    protected List<bool> isComplete;
    public List<bool> _IsComplete { get => isComplete; }

    private string nextQuestMsg;
    private List<int> clearNum;
    protected List<int> curClearNum;
    public List<int> _ClearNum { get => clearNum; }
    public List<int> _CurClearNum { get => curClearNum; }
    private List<string> clearInfo;
    public List<string> _ClearInfo { get => clearInfo; }

    private float curTime;
    private float timeLimit;
    public float _CurTime { get;}
    public float _TimeLimit { get;}
    public float _TimeRemain{ get { return timeLimit == 0 ? 1 : Mathf.Clamp(timeLimit - curTime, 0, timeLimit) / timeLimit; } }

    private bool isMainQuest = false;
    public bool _IsMainQuest { get => isMainQuest; }

    public abstract void CheckCondition();

    public virtual void CompleteQuest()
    {
        if (!string.IsNullOrEmpty(nextQuestMsg))
            QuestManager.Instance.EnqueueQuest(nextQuestMsg);
    }
    public abstract void FailQuest();

    public virtual void UpdateQuest()
    {
        if (!isComplete.Contains(false))
            return;

        CheckCondition();
        if(!isComplete.Contains(false))
        {
            CompleteQuest();
            return;
        }

        if(timeLimit != 0)
        {
            if (curTime > timeLimit && isComplete.Contains(false))
                FailQuest();
            curTime += Time.deltaTime * GameManager.Instance.DefaultSpeed * GameManager.Instance.timeScale;
        }
    }

    public void Init(List<Dictionary<string, object>> data)
    {
        questID = data[0]["ID"].ToString();
        timeLimit = float.Parse(data[0]["TimeLimit"].ToString());
        nextQuestMsg = data[0]["NextQuest"].ToString();
        isMainQuest = data[0]["Type"].ToString() == "main" ? true : false;
        curTime = 0;
        clearNum = new List<int>();
        curClearNum = new List<int>();
        clearInfo = new List<string>();
        foreach(Dictionary<string, object> val in data)
        {
            clearNum.Add(System.Convert.ToInt32(val["ClearNum"]));
            curClearNum.Add(0);
            clearInfo.Add(val["ClearInfo"].ToString());
            isComplete.Add(false);
        }
    }
}

public class Quest1001 : Quest
{
    public override void CheckCondition()
    {
        curClearNum[0] = _ClearNum[0] - Mathf.FloorToInt(_CurTime % 1440);
    }

    public override void CompleteQuest()
    {
        base.CompleteQuest();
        GameManager.Instance.gold -= 1000;
    }

    public override void FailQuest()
    {
        GameManager.Instance.LoseGame();
    }

    public override void UpdateQuest()
    {
        if (_CurTime > _TimeLimit)
        {
            if (GameManager.Instance.gold >= 1000)
                isComplete[0] = true;
        }
        base.UpdateQuest();
    }
}

public class Quest1002 : Quest
{
    public override void CheckCondition()
    {
        curClearNum[0] = _ClearNum[0] - Mathf.FloorToInt(_CurTime % 1440);
    }

    public override void CompleteQuest()
    {
        base.CompleteQuest();
        GameManager.Instance.gold -= 1000;
    }

    public override void FailQuest()
    {
        GameManager.Instance.LoseGame();
    }

    public override void UpdateQuest()
    {
        if (_CurTime > _TimeLimit)
        {
            if (GameManager.Instance.gold >= 1000)
                isComplete[0] = true;
        }
        base.UpdateQuest();
    }
}

public class Quest1003 : Quest
{
    public override void CheckCondition()
    {
        curClearNum[0] = _ClearNum[0] - Mathf.FloorToInt(_CurTime % 1440);
    }

    public override void CompleteQuest()
    {
        base.CompleteQuest();
        GameManager.Instance.gold -= 1000;
    }

    public override void FailQuest()
    {
        GameManager.Instance.LoseGame();
    }

    public override void UpdateQuest()
    {
        if (_CurTime > _TimeLimit)
        {
            if (GameManager.Instance.gold >= 1500)
                isComplete[0] = true;
        }
        base.UpdateQuest();
    }
}

public class Quest1004 : Quest
{
    public override void CheckCondition()
    {
        curClearNum[0] = _ClearNum[0] - Mathf.FloorToInt(_CurTime % 1440);
    }

    public override void CompleteQuest()
    {
        base.CompleteQuest();
        GameManager.Instance.gold -= 1000;
    }

    public override void FailQuest()
    {
        GameManager.Instance.LoseGame();
    }

    public override void UpdateQuest()
    {
        if (_CurTime > _TimeLimit)
        {
            if (GameManager.Instance.gold >= 1500)
                isComplete[0] = true;
        }
        base.UpdateQuest();
    }
}

public class Quest1005 : Quest
{
    public override void CheckCondition()
    {
        curClearNum[0] = _ClearNum[0] - Mathf.FloorToInt(_CurTime % 1440);
    }

    public override void CompleteQuest()
    {
        base.CompleteQuest();
        GameManager.Instance.gold -= 1000;
    }

    public override void FailQuest()
    {
        GameManager.Instance.LoseGame();
    }

    public override void UpdateQuest()
    {
        if (_CurTime > _TimeLimit)
        {
            if (GameManager.Instance.gold >= 3000)
                isComplete[0] = true;
        }
        base.UpdateQuest();
    }
}

public class Quest1006 : Quest
{
    public override void CheckCondition()
    {
        curClearNum[0] = _ClearNum[0] - Mathf.FloorToInt(_CurTime % 1440);
    }

    public override void CompleteQuest()
    {
        base.CompleteQuest();
        GameManager.Instance.gold -= 1000;
    }

    public override void FailQuest()
    {
        GameManager.Instance.LoseGame();
    }

    public override void UpdateQuest()
    {
        if (_CurTime > _TimeLimit)
        {
            if (GameManager.Instance.gold >= 3000)
                isComplete[0] = true;
        }
        base.UpdateQuest();
    }
}
