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

    [SerializeField]
    QuestMessage questMessage;
    [SerializeField]
    QuestController questController;

    List<string> clearedQuests = new List<string>();

    public void EndQuest(Quest quest, bool isClear)
    {
        //questController.EndQuest(quest, isClear);

        clearedQuests.Add(quest._QuestID);
    }

    public void StartQuest(string questID)
    {
        int id;
        if(int.TryParse(questID.Replace("q", ""), out id))
            questController.StartQuest(id);
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
            if (questMessage.gameObject.activeSelf || questWatcher.Count == 0 || GameManager.Instance.isPause)
            {
                yield return null;
                continue;
            }

            QuestWatcher watcher = questWatcher.Dequeue();
            if (GameManager.Instance.CurWave < watcher.startRound - 1 || !clearedQuests.Contains(watcher.condition))
                questWatcher.Enqueue(watcher);
            else
                questMessage.SetMessage(questMsgDic[watcher.id]);

            yield return null;
        }
    }

    private void Init()
    {
        if (initState)
            return;

        foreach(Dictionary<string, object> data in DataManager.Instance.QuestMessage_Table)
        {
            if (data["Condition"].ToString() == "-")
            {
                questWatcher.Enqueue(new QuestWatcher(data["ID"].ToString(), System.Convert.ToInt32(data["StartRound"]), data["Condition"].ToString()));
            }
            questMsgDic.Add(data["ID"].ToString(), data);
        }
        clearedQuests.Add("-");
    }

    private void Start()
    {
        Init();
        StartCoroutine(IQuestManage());
    }
}
