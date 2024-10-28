using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Quest1001 : Quest
{
    public override void CheckCondition()
    {
        curClearNum[0] = _ClearNum[0] - Mathf.FloorToInt(_CurTime / 1440);
    }

    public override void CompleteQuest()
    {
        base.CompleteQuest();
        GameManager.Instance.gold -= 500;
    }

    public override void FailQuest()
    {
        GameManager.Instance.LoseGame();
        base.FailQuest();
    }

    public override void UpdateQuest()
    {
        if (_CurTime > _TimeLimit)
        {
            if (GameManager.Instance.gold >= 500)
                isComplete[0] = true;
        }
        base.UpdateQuest();
    }
}

public class Quest1002 : Quest
{
    public override void CheckCondition()
    {
        curClearNum[0] = _ClearNum[0] - Mathf.FloorToInt(_CurTime / 1440);
    }

    public override void CompleteQuest()
    {
        base.CompleteQuest();
        GameManager.Instance.gold -= 800;
    }

    public override void FailQuest()
    {
        GameManager.Instance.LoseGame();
        base.FailQuest();
    }

    public override void UpdateQuest()
    {
        if (_CurTime > _TimeLimit)
        {
            if (GameManager.Instance.gold >= 800)
                isComplete[0] = true;
        }
        base.UpdateQuest();
    }
}

public class Quest1003 : Quest
{
    public override void CheckCondition()
    {
        curClearNum[0] = _ClearNum[0] - Mathf.FloorToInt(_CurTime / 1440);
    }

    public override void CompleteQuest()
    {
        base.CompleteQuest();
        GameManager.Instance.gold -= 1500;
    }

    public override void FailQuest()
    {
        GameManager.Instance.LoseGame();
        base.FailQuest();
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
        curClearNum[0] = _ClearNum[0] - Mathf.FloorToInt(_CurTime / 1440);
    }

    public override void CompleteQuest()
    {
        base.CompleteQuest();
        GameManager.Instance.gold -= 1800;
    }

    public override void FailQuest()
    {
        GameManager.Instance.LoseGame();
        base.FailQuest();
    }

    public override void UpdateQuest()
    {
        if (_CurTime > _TimeLimit)
        {
            if (GameManager.Instance.gold >= 1800)
                isComplete[0] = true;
        }
        base.UpdateQuest();
    }
}

public class Quest1005 : Quest
{
    public override void CheckCondition()
    {
        curClearNum[0] = _ClearNum[0] - Mathf.FloorToInt(_CurTime / 1440);
    }

    public override void CompleteQuest()
    {
        base.CompleteQuest();
        GameManager.Instance.gold -= 2500;
    }

    public override void FailQuest()
    {
        GameManager.Instance.LoseGame();
        base.FailQuest();
    }

    public override void UpdateQuest()
    {
        if (_CurTime > _TimeLimit)
        {
            if (GameManager.Instance.gold >= 2500)
                isComplete[0] = true;
        }
        base.UpdateQuest();
    }
}

public class Quest1006 : Quest
{
    public override void CheckCondition()
    {
        curClearNum[0] = _ClearNum[0] - Mathf.FloorToInt(_CurTime / 1440);
    }

    public override void CompleteQuest()
    {
        base.CompleteQuest();
        GameManager.Instance.gold -= 3500;
    }

    public override void FailQuest()
    {
        GameManager.Instance.LoseGame();
        base.FailQuest();
    }

    public override void UpdateQuest()
    {
        if (_CurTime > _TimeLimit)
        {
            if (GameManager.Instance.gold >= 3500)
                isComplete[0] = true;
        }
        base.UpdateQuest();
    }
}

public class Quest1007 : Quest
{
    public override void CheckCondition()
    {
        curClearNum[0] = _ClearNum[0] - Mathf.FloorToInt(_CurTime / 1440);
    }

    public override void CompleteQuest()
    {
        base.CompleteQuest();
        GameManager.Instance.gold -= 5000;
    }

    public override void FailQuest()
    {
        GameManager.Instance.LoseGame();
        base.FailQuest();
    }

    public override void UpdateQuest()
    {
        if (_CurTime > _TimeLimit)
        {
            if (GameManager.Instance.gold >= 5000)
                isComplete[0] = true;
        }
        base.UpdateQuest();
    }
}

public class Quest1008 : Quest
{
    public override void CheckCondition()
    {
        curClearNum[0] = _ClearNum[0] - Mathf.FloorToInt(_CurTime / 1440);
    }

    public override void CompleteQuest()
    {
        QuestManager.Instance.EndQuest(this, true);
        GameManager.Instance.gold -= 1000000;
        GameManager.Instance.WinGame();
    }

    public override void FailQuest()
    {
        base.FailQuest();
        if (!string.IsNullOrEmpty(nextQuestMsg))
            QuestManager.Instance.EnqueueQuest(nextQuestMsg);
    }

    public override void UpdateQuest()
    {
        if (_CurTime > _TimeLimit)
        {
            if (GameManager.Instance.gold >= 1000000)
                isComplete[0] = true;
        }
        base.UpdateQuest();
    }
}

public class Quest1009 : Quest
{
    public override void CheckCondition() { }

    public override void UpdateQuest()
    {
        isComplete[0] = true;
        base.UpdateQuest();
    }
}

public class Quest1010 : Quest
{
    public override void CheckCondition() { }

    public override void UpdateQuest()
    {
        isComplete[0] = true;
        base.UpdateQuest();
    }
}

public class Quest1011 : Quest
{
    private Adventurer boss = null;

    private Adventurer _Boss
    {
        get
        {
            if (boss == null)
            {
                foreach (Adventurer target in GameManager.Instance.adventurersList)
                {
                    if (target.BattlerID == "s_a99001")
                    {
                        boss = target;
                        break;
                    }
                }
            }
            return boss;
        }
    }

    public override void CheckCondition()
    {
        curClearNum[0] = _ClearNum[0] - Mathf.FloorToInt(_CurTime / 1440);

        if (_Boss == null)
            return;

        if (_Boss.isDead)
            isComplete[0] = true;
    }

    public override void CompleteQuest()
    {
        base.CompleteQuest();
        GameManager.Instance.WinGame();
    }

    public override void FailQuest()
    {
        GameManager.Instance.LoseGame();
        base.FailQuest();
    }
}

public class Quest1012 : Quest
{
    private Adventurer boss = null;

    private Adventurer _Boss
    {
        get
        {
            if (boss == null)
            {
                foreach (Adventurer target in GameManager.Instance.adventurersList)
                {
                    if (target.BattlerID == "s_a99001")
                    {
                        boss = target;
                        break;
                    }
                }
            }
            return boss;
        }
    }

    public override void CheckCondition()
    {
        curClearNum[0] = _ClearNum[0] - Mathf.FloorToInt(_CurTime / 1440);

        if (_Boss == null)
            return;

        if (_Boss.isDead)
            isComplete[0] = true;
    }

    public override void CompleteQuest()
    {
        base.CompleteQuest();
        GameManager.Instance.WinGame();
    }

    public override void FailQuest()
    {
        GameManager.Instance.LoseGame();
        base.FailQuest();
    }
}
