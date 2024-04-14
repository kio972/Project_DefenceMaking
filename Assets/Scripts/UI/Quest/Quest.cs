using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Quest
{
    protected bool isComplete = false;

    private string nextQuestMsg;
    private int clearNum;
    private int curClearNum;
    public int _ClearNum { get => clearNum; }
    public int _CurClearNum { get => curClearNum; }

    private float curTime;
    private float timeLimit;
    public float _CurTime { get;}
    public float _TimeLimit { get;}
    public float _TimeRemain{ get { return timeLimit == 0 ? 1 : Mathf.Clamp(timeLimit - curTime, 0, timeLimit) / timeLimit; } }

    public abstract void CheckCondition();

    public abstract void CompleteQuest();
    public abstract void FailQuest();

    public void UpdateQuest()
    {
        if (isComplete)
            return;

        CheckCondition();
        if(curClearNum >= clearNum)
        {
            CompleteQuest();
            isComplete = true;
            if (!string.IsNullOrEmpty(nextQuestMsg))
                QuestManager.Instance.EnqueueQuest(nextQuestMsg);
            return;
        }

        if(timeLimit != 0)
        {
            curTime += Time.deltaTime * GameManager.Instance.DefaultSpeed * GameManager.Instance.timeScale;
            if (curTime > timeLimit)
                FailQuest();
        }
    }

    public void Init(float timeLimit, int clearNum, string nextQuestMsg)
    {
        curTime = 0;
        this.timeLimit = timeLimit;
        this.clearNum = clearNum;
        this.nextQuestMsg = nextQuestMsg;
    }
}
