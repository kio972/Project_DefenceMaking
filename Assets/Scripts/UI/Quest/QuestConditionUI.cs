using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class QuestConditionUI : MonoBehaviour
{
    [SerializeField]
    private GameObject checkBox;
    [SerializeField]
    private TextMeshProUGUI text;

    public Quest quest = null;
    private int index = 0;
    private bool isClear = false;

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
        text.color = Color.white;
        text.fontStyle = FontStyles.Normal;
        checkBox.SetActive(false);
        isClear = false;
    }

    void Update()
    {
        if (quest == null || isClear)
            return;

        SetText();
        if(quest._IsComplete[index])
        {
            checkBox.SetActive(true);
            text.fontStyle = FontStyles.Strikethrough;
            text.color = Color.gray;
            isClear = true;
        }
    }
}
