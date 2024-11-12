using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestDebtRepay : Quest
{
    protected int targetGold;
    protected int executorCount;

    public override void CheckCondition()
    {
        curClearNum[1] = _ClearNum[1] - Mathf.FloorToInt(_CurTime / 1440);
        if (curClearNum[0] >= targetGold)
            isComplete[0] = true;
    }

    protected async UniTaskVoid SpawnExecutor(int count)
    {
        DebtRepay repay = null;
        foreach(var item in GameManager.Instance.shop.itemSlots)
        {
            if(item.item is DebtRepay repayItem && repayItem.TargetDebtQuestId == _QuestID)
                repay = repayItem;
        }

        for (int i = 1; i <= count; i++)
        {
            bool debtPayed = repay != null && repay.itemSlot.curStockCount <= 0;
            if (debtPayed)
                return;

            BattlerPooling.Instance.SpawnAdventurer("executor_Lv1");
            if (count == i)
                break;

            float elpasedTime = 0;
            while (elpasedTime <= 120)
            {
                elpasedTime += GameManager.Instance.InGameDeltaTime;
                await UniTask.Yield(cancellationToken: GameManager.Instance.gameObject.GetCancellationTokenOnDestroy());
            }
        }
    }

    public override void FailQuest()
    {
        SpawnExecutor(1).Forget();
        base.FailQuest();
        if (!string.IsNullOrEmpty(nextQuestMsg))
            QuestManager.Instance.EnqueueQuest(nextQuestMsg);
    }

    public void ReduceGold(int amount)
    {
        curClearNum[0] += amount;
    }

    public override void UpdateQuest()
    {
        if (_CurTime > _TimeLimit)
            isComplete[1] = true;

        base.UpdateQuest();
    }
}

public class Quest1001 : QuestDebtRepay
{
    public Quest1001()
    {
        targetGold = 500;
        executorCount = 1;
    }
}

public class Quest1002 : QuestDebtRepay
{
    public Quest1002()
    {
        targetGold = 1000;
        executorCount = 2;
    }
}

public class Quest1003 : QuestDebtRepay
{
    public Quest1003()
    {
        targetGold = 2500;
        executorCount = 3;
    }
}

public class Quest1004 : QuestDebtRepay
{
    public Quest1004()
    {
        targetGold = 4000;
        executorCount = 4;
    }
}

public class Quest1005 : QuestDebtRepay
{
    public Quest1005()
    {
        targetGold = 7000;
        executorCount = 5;
    }
}

public class Quest1006 : QuestDebtRepay
{
    public Quest1006()
    {
        targetGold = 1000000;
    }

    public override void CompleteQuest()
    {
        QuestManager.Instance.EndQuest(this, true);
        GameManager.Instance.WinGame();
    }

    public override void FailQuest()
    {
        base.FailQuest();
        if (!string.IsNullOrEmpty(nextQuestMsg))
            QuestManager.Instance.EnqueueQuest(nextQuestMsg);
    }
}

public class Quest1007 : Quest
{
    public override void CheckCondition() { }

    public override void UpdateQuest()
    {
        isComplete[0] = true;
        base.UpdateQuest();
    }
}

public class Quest1008 : Quest
{
    public override void CheckCondition() { }

    public override void UpdateQuest()
    {
        isComplete[0] = true;
        base.UpdateQuest();
    }
}

public class Quest1009 : Quest
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

public class Quest1010 : Quest
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

public class Quest1011 : Quest
{
    public override void CheckCondition()
    {
        
    }
}
