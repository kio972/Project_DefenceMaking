using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

public class DebtRepay : MonoBehaviour, Item, INeedUnlockItem
{
    [SerializeField]
    private ShopUI shopUI;

    [SerializeField]
    private string descScript;
    [SerializeField]
    private string buyScript;

    [SerializeField]
    private string targetDebtQuestId;

    ItemSlot itemSlot;
    ItemSlot _ItemSlot
    {
        get
        {
            if (itemSlot == null)
                itemSlot = GetComponent<ItemSlot>();
            return itemSlot;
        }
    }

    public bool IsUnlock
    {
        get
        {
            if(QuestManager.Instance.questController.mainQuest != null && QuestManager.Instance.questController.mainQuest._QuestID == targetDebtQuestId)
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

        string conver = script["script"].ToString();
        string track0 = script["track0"].ToString();
        string track1 = script["track1"].ToString();
        string[] convers = conver.Split('%');
        shopUI?.PlayScript(convers[0] + "\'" + money.ToString() + "\'" + convers[1], track0, track1);
    }

    public void UseItem()
    {
        //����Ʈ���� �ش� ������ ����(SoldOut�����, ����Ҵ���)
    }
}