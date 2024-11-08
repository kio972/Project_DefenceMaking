using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Quest
{
    private string questID;
    private string questName;
    public string _QuestName { get => questName; }
    public string _QuestID { get => questID; }
    protected List<bool> isComplete;
    public List<bool> _IsComplete { get => isComplete; }

    protected string nextQuestMsg;
    private List<int> clearNum;
    protected List<int> curClearNum;
    public List<int> _ClearNum { get => clearNum; }
    public List<int> _CurClearNum { get => curClearNum; }
    private List<string> clearInfo;
    public List<string> _ClearInfo { get => clearInfo; }

    private float curTime;
    private float timeLimit;
    public float _CurTime { get => curTime; }
    public float _TimeLimit { get => timeLimit; }
    public float _TimeRemain { get { return timeLimit == 0 ? 1 : Mathf.Clamp(timeLimit - curTime, 0, timeLimit) / timeLimit; } }

    private bool isMainQuest = false;
    public bool _IsMainQuest { get => isMainQuest; }

    protected bool isEnd = false;
    public bool _IsEnd { get => isEnd; }
    public bool _IsClear { get { return isEnd && !isComplete.Contains(false) ? true : false; } }

    private string targetReward;
    private int rewardValue;

    public abstract void CheckCondition();

    protected virtual void GetReward()
    {
        if (string.IsNullOrEmpty(targetReward))
            return;

        switch(targetReward)
        {
            case "Gold":
                GameManager.Instance.gold += rewardValue;
                break;
            case "BlackHerb":
                GameManager.Instance.herbDic[HerbType.BlackHerb] += rewardValue;
                break;
            case "PurpleHerb":
                GameManager.Instance.herbDic[HerbType.PurpleHerb] += rewardValue;
                break;
            case "WhiteHerb":
                GameManager.Instance.herbDic[HerbType.BlackHerb] += rewardValue;
                break;
        }
    }

    public virtual void CompleteQuest()
    {
        for (int i = 0; i < isComplete.Count; i++)
            isComplete[i] = true;

        GetReward();

        if (!string.IsNullOrEmpty(nextQuestMsg))
            QuestManager.Instance.EnqueueQuest(nextQuestMsg);
        QuestManager.Instance.EndQuest(this, true);
    }
    public virtual void FailQuest()
    {
        QuestManager.Instance.EndQuest(this, false);
    }

    public virtual void UpdateQuest()
    {
        if (isEnd)
            return;

        CheckCondition();
        if(!isComplete.Contains(false))
        {
            CompleteQuest();
            isEnd = true;
            return;
        }

        if(timeLimit != 0)
        {
            if (curTime > timeLimit && isComplete.Contains(false))
            {
                FailQuest();
                isEnd = true;
            }
            curTime += GameManager.Instance.InGameDeltaTime;
        }
    }

    public void Init(List<Dictionary<string, object>> data, List<int> startVal = null)
    {
        questID = data[0]["ID"].ToString();
        questName = data[0]["Name"].ToString();
        timeLimit = float.Parse(data[0]["TimeLimit"].ToString());
        nextQuestMsg = data[0]["NextQuest"].ToString();
        isMainQuest = data[0]["Type"].ToString() == "main" ? true : false;
        targetReward = data[0]["Reward"].ToString();
        if (int.TryParse(data[0]["RewardNum"].ToString(), out int rewardNum))
            rewardValue = rewardNum;

        curTime = 0;
        clearNum = new List<int>();
        curClearNum = startVal != null ? new List<int>(startVal) : new List<int>();
        clearInfo = new List<string>();
        isComplete = new List<bool>();
        foreach(Dictionary<string, object> val in data)
        {
            clearNum.Add(System.Convert.ToInt32(val["ClearNum"]));
            if(startVal == null)
                curClearNum.Add(0);
            clearInfo.Add(val["ClearInfo"].ToString());
            isComplete.Add(false);
        }
    }
}
