using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

public class DebtRepay : MonoBehaviour, Item, INeedUnlockItem
{
    [SerializeField]
    private ShopUI shopUI;


    [SerializeField]
    private string buyScript;
    [SerializeField]
    private string soldOutScript;

    [SerializeField]
    private string targetDebtQuestId;

    public string TargetDebtQuestId;

    ItemSlot _itemSlot;
    public ItemSlot itemSlot
    {
        get
        {
            if (_itemSlot == null)
                _itemSlot = GetComponent<ItemSlot>();
            return _itemSlot;
        }
    }

    public bool IsUnlock
    {
        get
        {
            if(QuestManager.Instance.questController.mainQuest != null && QuestManager.Instance.questController.mainQuest._QuestID == targetDebtQuestId)
                return true;
            if (QuestManager.Instance.IsQuestFailed(targetDebtQuestId) && itemSlot.curStockCount > 0)
                return true;
            return false;
        }
    }

    private int GetRandomIndex(List<int> target)
    {
        int randomIndex = Random.Range(0, target.Count);

        return target[randomIndex];
    }

    public void PlayBuyScript(int money)
    {
        Dictionary<string, object> script = DataManager.Instance.GetMalpoongsunScript(buyScript);
        if (script == null)
            return;

        string conver = script[SettingManager.Instance.language.ToString()].ToString();
        string track0 = script["track0"].ToString();
        string track1 = script["track1"].ToString();
        string[] convers = conver.Split('%');
        shopUI?.PlayScript(convers[0] + "\'" + money.ToString() + "\'" + convers[1], track0, track1);
    }

    public void UseItem()
    {
        //퀘스트에서 해당 아이템 감시(SoldOut됬는지, 몇개남았는지)
        if(QuestManager.Instance.questController.mainQuest is QuestDebtRepay repay && repay._QuestID == targetDebtQuestId)
        {
            repay.ReduceGold(itemSlot._CurPrice);
        }

        //모두 구매했을 때, 이미 퀘스트를 실패한 상태라면 집행관들을 모두 돌려보낸다
        if(_itemSlot.curStockCount <= 0 && QuestManager.Instance.IsQuestFailed(targetDebtQuestId))
        {
            List<Adventurer> executors = new List<Adventurer>();
            foreach(var adventurer in GameManager.Instance.adventurersList)
            {
                if(adventurer.BattlerID == "s_boss006")
                    executors.Add(adventurer);
            }
            foreach (var executor in executors)
                executor.ReturnToBase(false);
        }

        if(_itemSlot.curStockCount <= 0)
            shopUI?.PlayScript(soldOutScript);
        else
            shopUI?.PlayScript(buyScript);
    }
}
