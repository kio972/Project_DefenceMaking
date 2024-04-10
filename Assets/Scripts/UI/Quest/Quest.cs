using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Quest
{

    private bool isComplete = false;

    public float _CurTime { get; set; }
    public float _TimeLimit { get; set; }
    public float _TimeRemain { get { return Mathf.Clamp(_TimeLimit - _CurTime, 0, _TimeLimit) / _TimeLimit; } }

    public abstract void CheckCondition();

    public abstract void CompleteQuest();
    public abstract void FailQuest();

    public void UpdateQuest()
    {
        if (isComplete)
            return;

        CheckCondition();
        if(isComplete)
        {
            CompleteQuest();
            return;
        }

        _CurTime += Time.deltaTime * GameManager.Instance.DefaultSpeed * GameManager.Instance.timeScale;
        if(_CurTime > _TimeLimit)
            FailQuest();
    }
}
