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

    private Dictionary<int, Dictionary<string, object>> questDic;
    private Dictionary<int, Dictionary<string, object>> _QuestDic
    {
        get
        {
            if(questDic == null)
            {
                questDic = new Dictionary<int, Dictionary<string, object>>();
                foreach (Dictionary<string, object> data in DataManager.Instance.Quest_Table)
                {
                    int index;
                    if(int.TryParse(data["ID"].ToString().Replace("q", ""), out index))
                        questDic.Add(index, data);
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

    public void StartQuest(int questID)
    {
        Quest curQuest = LoadQuest(questID);
        if (curQuest == null)
            return;
        curQuest.Init(float.Parse(_QuestDic[questID]["TimeLimit"].ToString()), System.Convert.ToInt32(_QuestDic[questID]["ClearNum"]), _QuestDic[questID]["NextQuest"].ToString());
        if (_QuestDic[questID]["Type"].ToString() == "main")
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
