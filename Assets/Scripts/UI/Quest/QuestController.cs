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

    private Quest mainQuest;
    private List<Quest> subQuest = new List<Quest>();

    private Quest curQuest;

    private Quest LoadQuest(int questID)
    {
        string questClassName = "Quest" + questID.ToString(); // 퀘스트 클래스의 이름
        Type questType = Type.GetType(questClassName);

        if (questType != null && typeof(Quest).IsAssignableFrom(questType))
            return (Quest)Activator.CreateInstance(questType);
        else
            return null;
    }


    public void StartQuest(int questID)
    {
        curQuest = LoadQuest(questID);
        if (curQuest == null)
            return;

        curQuest._TimeLimit = 1440;
        mainInfomer.SetQuest(curQuest);
    }

    private void Update()
    {
        if (curQuest == null)
            return;

        curQuest.UpdateQuest();
        
    }

    private void Start()
    {
        StartQuest(00);
    }
}
