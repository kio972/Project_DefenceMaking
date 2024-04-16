using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct QuestData
{

}

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
            foreach(QuestInfo questInfo in subInformer)
            {
                if (!questInfo.gameObject.activeSelf)
                    return questInfo;
            }

            QuestInfo newInfo = Instantiate(Resources.Load<QuestInfo>("Prefab/UI/QuestInfoSub"), transform);
            return newInfo;
        }
    }

    private Quest mainQuest;
    private List<Quest> subQuest = new List<Quest>();

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

    private Quest LoadQuest(int questID)
    {
        string questClassName = "Quest" + questID.ToString(); // 퀘스트 클래스의 이름
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
        nextSubInformer.SetQuest(quest);
    }

    public void EndQuest(Quest quest, bool isClear)
    {

    }

    public void StartQuest(int questID)
    {
        Quest curQuest = LoadQuest(questID);
        if (curQuest == null)
            return;
        curQuest.Init(_QuestDic[questID]);
        if (curQuest._IsMainQuest)
            SetMainQuest(curQuest);
        else
            SetSubQuest(curQuest);
    }

    private void Update()
    {
        mainQuest?.UpdateQuest();
        foreach(Quest quest in subQuest)
            quest.UpdateQuest();
    }
}
