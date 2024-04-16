using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class QuestCondition : MonoBehaviour
{
    [SerializeField]
    private GameObject checkBox;
    [SerializeField]
    private TextMeshProUGUI text;

    public Quest quest = null;
    private int index = 0;

    public void SetText()
    {
        if (quest._ClearNum[index] > 0)
            text.text = $"{quest._ClearInfo[index]}{" (남은 라운드 "}{quest._CurClearNum[index]}{")"}";
        else if (quest._ClearNum[index] < 0)
            text.text = $"{quest._ClearInfo[index]}{" ("}{quest._CurClearNum[index]}{" / "}{Mathf.Abs(quest._ClearNum[index])}{")"}";
        else
            text.text = quest._ClearInfo[index];
    }

    public void SetQuest(Quest quest, int index)
    {
        this.quest = quest;
        this.index = index;
    }

    void Update()
    {
        if (quest == null)
            return;

        SetText();
    }
}
