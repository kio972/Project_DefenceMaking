using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct QuestWatcher
{
    public string id;
    public int startRound;
    public string condition;

    public QuestWatcher(string id, int startRound, string condition)
    {
        this.id = id;
        this.startRound = startRound;
        this.condition = condition;
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
    QuestController questController;
    QuestController _QuestController
    {
        get
        {
            if (questController == null)
                questController = FindObjectOfType<QuestController>(true);
            return questController;
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
            _QuestController.StartQuest(id, startVal);
    }

    public void EnqueueQuest(string msgID)
    {
        if (!questMsgDic.ContainsKey(msgID))
            return;

        Dictionary<string, object> data = questMsgDic[msgID];
        questWatcher.Enqueue(new QuestWatcher(data["ID"].ToString(), System.Convert.ToInt32(data["StartRound"]), data["Condition"].ToString()));
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

            QuestWatcher watcher = questWatcher.Dequeue();
            if (GameManager.Instance.CurWave < watcher.startRound - 1 || !clearedQuests.Contains(watcher.condition))
                questWatcher.Enqueue(watcher);
            else
                _QuestMessage.SetMessage(questMsgDic[watcher.id]);

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
            if (autoEnqueue && data["Condition"].ToString() == "-")
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
        main.id = questController._MainQuest._QuestID;
        main.curVal = questController._MainQuest._CurClearNum;
        data.curQuests.Add(main);

        foreach (Quest quest in questController._SubQuest)
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
