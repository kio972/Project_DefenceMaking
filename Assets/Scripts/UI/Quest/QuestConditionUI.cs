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

    private int curClearNum = -1;

    public void SetText()
    {
        if (quest._ClearNum[index] > 0)
            text.text = $"{DataManager.Instance.GetDescription(quest._ClearInfo[index])} ({DataManager.Instance.GetDescription("q_ui_rounds")} : {quest._CurClearNum[index]})";
        else if (quest._ClearNum[index] < 0)
            text.text = $"{DataManager.Instance.GetDescription(quest._ClearInfo[index])} ({quest._CurClearNum[index]}{" / "}{Mathf.Abs(quest._ClearNum[index])})";
        else
            text.text = DataManager.Instance.GetDescription(quest._ClearInfo[index]);
    }

    public void SetQuest(Quest quest, int index)
    {
        this.quest = quest;
        this.index = index;
        text.color = Color.white;
        text.fontStyle = FontStyles.Normal;
        if(checkBox != null)
            checkBox.SetActive(false);
        isClear = false;
    }

    void Update()
    {
        if (quest == null || isClear)
            return;

        if(curClearNum != quest._CurClearNum[index])
        {
            curClearNum = quest._CurClearNum[index];
            SetText();
        }

        if(quest._IsComplete[index])
        {
            if (checkBox != null)
                checkBox.SetActive(true);
            text.fontStyle = FontStyles.Strikethrough;
            text.color = Color.gray;
            isClear = true;
        }
    }

    private void Start()
    {
        LanguageManager.Instance.AddLanguageAction(() => SetText());
    }
}
