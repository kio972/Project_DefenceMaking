using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct QuestWatcher
{
    public string id;
    public int startRound;
    public string condition;
    public QuestCondition customCondition;

    private QuestCondition CustomCondition(string id)
    {
        id = id.Replace("q_", "QusetCondition_");
        Type questType = Type.GetType(id);

        if (questType != null && typeof(QuestCondition).IsAssignableFrom(questType))
            return (QuestCondition)Activator.CreateInstance(questType);
        else
            return null;
    }

    public QuestWatcher(string id, int startRound, string condition)
    {
        this.id = id;
        this.startRound = startRound;
        this.condition = condition;
        customCondition = null;
        if(this.condition == "custom")
            customCondition = CustomCondition(id);
    }
}

public class QuestManager : IngameSingleton<QuestManager>
{
    Queue<QuestWatcher> questWatcher = new Queue<QuestWatcher>();

    Dictionary<string, Dictionary<string, object>> questMsgDic = new Dictionary<string, Dictionary<string, object>>();

    private bool initState = false;

    QuestMessage questMessage;
    QuestMessage _QuestMessage
    {
        get
        {
            if (questMessage == null)
                questMessage = FindObjectOfType<QuestMessage>(true);
            return questMessage;
        }
    }
    QuestController _questController;
    public QuestController questController
    {
        get
        {
            if (_questController == null)
                _questController = FindObjectOfType<QuestController>(true);
            return _questController;
        }
    }

    HashSet<string> clearedQuests = new HashSet<string>();



    public void EndQuest(Quest quest, bool isClear)
    {
        //questController.EndQuest(quest, isClear);

        clearedQuests.Add(quest._QuestID);
    }

    public void StartQuest(string questID, List<int> startVal = null)
    {
        int id;
        if(int.TryParse(questID.Replace("q", ""), out id))
            questController.StartQuest(id, startVal);
    }

    public void EnqueueQuest(string msgID)
    {
        if (!questMsgDic.ContainsKey(msgID))
            return;

        Dictionary<string, object> data = questMsgDic[msgID];
        questWatcher.Enqueue(new QuestWatcher(data["ID"].ToString(), System.Convert.ToInt32(data["StartRound"]), data["Condition"].ToString()));
    }

    private bool IsQuestConditionPassed(QuestWatcher questWatcher)
    {
        if (GameManager.Instance.CurWave < questWatcher.startRound - 1)
            return false;

        if (questWatcher.condition == "custom")
            return questWatcher.customCondition.IsConditionPassed();

        if (string.IsNullOrEmpty(questWatcher.condition) && questWatcher.condition != "-")
            if (!clearedQuests.Contains(questWatcher.condition))
                return false;

        return true;
    }

    private IEnumerator IQuestManage()
    {
        while(true)
        {
            if (_QuestMessage.gameObject.activeSelf || questWatcher.Count == 0 || GameManager.Instance.isPause)
            {
                yield return null;
                continue;
            }

            QuestWatcher watcher = questWatcher.Peek();
            bool isStartable = IsQuestConditionPassed(watcher);
            if (!isStartable)
            {
                questWatcher.Dequeue();
                questWatcher.Enqueue(watcher);
            }
            else
            {
                _QuestMessage.SetMessage(questMsgDic[watcher.id]).Forget();
                while(_QuestMessage.gameObject.activeSelf)
                    yield return null;
                questWatcher.Dequeue();
            }

            yield return null;
        }
    }

    public void InitQuest(bool autoEnqueue = true)
    {
        if (initState)
            return;
        initState = true;
        foreach (Dictionary<string, object> data in DataManager.Instance.QuestMessage_Table)
        {
            questMsgDic.Add(data["ID"].ToString(), data);
            if (autoEnqueue && (data["Condition"].ToString() is "-" or "custom"))
                EnqueueQuest(data["ID"].ToString());
        }
        clearedQuests.Add("-");

        StartCoroutine(IQuestManage());
    }

    private void Update()
    {
        if (initState)
            return;
        InitQuest();
    }

    public void SaveGame(PlayerData data)
    {
        data.curQuests = new List<QuestData>();
        QuestData main = new QuestData();
        if(questController._MainQuest != null)
        {
            main.id = _questController._MainQuest._QuestID;
            main.curVal = _questController._MainQuest._CurClearNum;
            data.curQuests.Add(main);
        }

        foreach (Quest quest in _questController._SubQuest)
        {
            QuestData sub = new QuestData();
            sub.id = quest._QuestID;
            sub.curVal = quest._CurClearNum;
            data.curQuests.Add(sub);
        }

        data.enqueuedQuests = new List<string>();
        foreach(QuestWatcher watcher in questWatcher)
            data.enqueuedQuests.Add(watcher.id);

        data.clearedQuests = new List<string>(clearedQuests);
    }

    public void LoadGame(PlayerData data)
    {
        InitQuest(false);

        foreach(QuestData quest in data.curQuests)
            StartQuest(quest.id, quest.curVal);
        foreach (string quest in data.clearedQuests)
            clearedQuests.Add(quest);

        foreach (string quest in data.enqueuedQuests)
            EnqueueQuest(quest);
    }
}
