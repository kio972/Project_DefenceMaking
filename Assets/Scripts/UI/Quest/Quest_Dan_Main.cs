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

