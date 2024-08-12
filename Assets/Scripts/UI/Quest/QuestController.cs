using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestController : MonoBehaviour
{
    [SerializeField]
    QuestInfo mainInfomer;
    [SerializeField]
    List<QuestInfo> subInformer;

    QuestInfo nextSubInformer
    {
        get
        {
            foreach (QuestInfo questInfo in subInformer)
            {
                if (!questInfo.gameObject.activeSelf)
                    return questInfo;
            }

            QuestInfo newInfo = Instantiate(Resources.Load<QuestInfo>("Prefab/UI/QuestInfoSub"), transform);
            return newInfo;
        }
    }

    private Quest mainQuest;
    public Quest _MainQuest { get => mainQuest; }

    private List<Quest> subQuest = new List<Quest>();
    public List<Quest> _SubQuest { get => subQuest; }

    private Dictionary<int, List<Dictionary<string, object>>> questDic;
    private Dictionary<int, List<Dictionary<string, object>>> _QuestDic
    {
        get
        {
            if(questDic == null)
            {
                questDic = new Dictionary<int, List<Dictionary<string, object>>>();
                foreach (Dictionary<string, object> data in DataManager.Instance.Quest_Table)
                {
                    int index;
                    if(int.TryParse(data["ID"].ToString().Replace("q", ""), out index))
                    {
                        if (!questDic.ContainsKey(index))
                            questDic.Add(index, new List<Dictionary<string, object>>());
                        questDic[index].Add(data);
                    }
                }
            }
            return questDic;
        }
    }

    [SerializeField]
    private VerticalLayoutGroup layoutGroup;

    private Quest LoadQuest(int questID)
    {
        string questClassName = "Quest" + questID.ToString(); // ����Ʈ Ŭ������ �̸�
        Type questType = Type.GetType(questClassName);

        if (questType != null && typeof(Quest).IsAssignableFrom(questType))
            return (Quest)Activator.CreateInstance(questType);
        else
            return null;
    }

    private void SetMainQuest(Quest quest)
    {
        mainQuest = quest;
        mainInfomer.SetQuest(quest);
    }

    private void SetSubQuest(Quest quest)
    {
        subQuest.Add(quest);
        QuestInfo next = nextSubInformer;
        next.SetQuest(quest);
        layoutGroup.enabled = false;
        next.transform.SetAsLastSibling();
        layoutGroup.enabled = true;
    }

    public void StartQuest(int questID, List<int> startVal)
    {
        Quest curQuest = LoadQuest(questID);
        if (curQuest == null)
            return;
        curQuest.Init(_QuestDic[questID], startVal);
        if (curQuest._IsMainQuest)
            SetMainQuest(curQuest);
        else
            SetSubQuest(curQuest);
    }

    private void Update()
    {
        mainQuest?.UpdateQuest();
        List<Quest> clearedQuest = new List<Quest>();
        foreach(Quest quest in subQuest)
        {
            quest.UpdateQuest();
            if (quest._IsEnd)
                clearedQuest.Add(quest);
        }

        foreach (Quest quest in clearedQuest)
            subQuest.Remove(quest);
    }
}
