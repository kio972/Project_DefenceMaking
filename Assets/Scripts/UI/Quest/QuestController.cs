using System;
using System.Collections;
using System.Collections.Generic;
using FMODUnity;
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

    private Quest _mainQuest;
    public Quest mainQuest { get => _mainQuest; }

    private List<Quest> _subQuest = new List<Quest>();
    public List<Quest> subQuest { get => _subQuest; }

    private Dictionary<int, List<Dictionary<string, object>>> questDic;
    private Dictionary<int, List<Dictionary<string, object>>> _QuestDic
    {
        get
        {
            if(questDic == null)
            {
                questDic = new Dictionary<int, List<Dictionary<string, object>>>();
                foreach (Dictionary<string, object> data in DataManager.Instance.quest_Table)
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

    [SerializeField]
    FMODUnity.EventReference createSound;

    public QuestInfo GetInformer(string questId)
    {
        if (mainInfomer.curQuest._QuestID == questId)
            return mainInfomer;

        foreach(QuestInfo questInfo in subInformer)
        {
            if (questInfo.curQuest == null)
                continue;

            if(questInfo.curQuest._QuestID == questId)
                return questInfo;
        }

        return null;
    }

    public bool IsQuestStarted(string questID)
    {
        if (_mainQuest != null && _mainQuest._QuestID == questID)
            return true;

        foreach (var quest in _subQuest)
            if (quest._QuestID == questID)
                return true;

        return false;
    }

    private Quest LoadQuest(int questID)
    {
        string questClassName = "Quest" + questID.ToString("0000"); // 퀘스트 클래스의 이름
        Type questType = Type.GetType(questClassName);

        if (questType != null && typeof(Quest).IsAssignableFrom(questType))
            return (Quest)Activator.CreateInstance(questType);
        else
            return null;
    }

    private void SetMainQuest(Quest quest)
    {
        _mainQuest = quest;
        mainInfomer.SetQuest(quest);

        //AudioManager.Instance.Play2DSound("Quset_Creat_02", SettingManager.Instance._FxVolume);
        RuntimeManager.PlayOneShot(createSound);
    }

    private void SetSubQuest(Quest quest)
    {
        _subQuest.Add(quest);
        QuestInfo next = nextSubInformer;
        next.SetQuest(quest);
        layoutGroup.enabled = false;
        next.transform.SetAsLastSibling();
        layoutGroup.enabled = true;

        //AudioManager.Instance.Play2DSound("Quset_Creat_01", SettingManager.Instance._FxVolume);
        RuntimeManager.PlayOneShot(createSound);
    }

    public void StartQuest(int questID, List<int> startVal, float startTime)
    {
        Quest curQuest = LoadQuest(questID);
        if (curQuest == null)
            return;
        curQuest.Init(_QuestDic[questID], startVal, startTime);
        if (curQuest._IsMainQuest)
            SetMainQuest(curQuest);
        else
            SetSubQuest(curQuest);
    }

    private void Update()
    {
        _mainQuest?.UpdateQuest();
        List<Quest> clearedQuest = new List<Quest>();
        foreach(Quest quest in _subQuest)
        {
            quest.UpdateQuest();
            if (quest._IsEnd)
                clearedQuest.Add(quest);
        }

        foreach (Quest quest in clearedQuest)
            _subQuest.Remove(quest);
    }
}
