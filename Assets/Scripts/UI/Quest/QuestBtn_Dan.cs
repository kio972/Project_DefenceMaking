using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestBtn_Dan : MouseOverTarget, IQuestInteract
{
    QuestDebtRepay repay;
    int rate = 10;
    int repayAmount;

    Button interactBtn;

    [SerializeField]
    FMODUnity.EventReference successClipName;
    [SerializeField]
    FMODUnity.EventReference faillClipName;

    readonly Vector2 pivot = new Vector2(0f, 1.4f);
    readonly string interactBtnKey = "tooltip_ui_debt1_0";
    
    public void EndQuest()
    {
        repay = null;
        gameObject.SetActive(false);
    }

    public void OnClick()
    {
        if (repay == null || repay._IsClear)
            return;

        if (repayAmount > GameManager.Instance.gold)
        {
            FMODUnity.RuntimeManager.PlayOneShot(faillClipName);
            return;
        }

        GameManager.Instance.gold -= repayAmount;
        repay.ReduceGold(repayAmount);
        FMODUnity.RuntimeManager.PlayOneShot(successClipName);
        repay.UpdateQuest();
        if (repay._IsComplete[0])
            gameObject.SetActive(false);
    }

    public override void SetText()
    {
        string header = DataManager.Instance.GetDescription("tooltip_ui_debt1_0");
        string desc = DataManager.Instance.GetDescription("tooltip_ui_debt1_1");
        string additional = $"{DataManager.Instance.GetDescription("ui_GoldCost")} : <color=yellow>{repayAmount}</color> <sprite name=\"Gold\">";
        GameManager.Instance._InGameUI.mouseOverTooltip?.SetMesseage(transform, pivot, true, header, desc, additional);
    }

    public void SetQuest(Quest quest)
    {
        if(interactBtn != null)
            interactBtn.onClick.RemoveAllListeners();
        interactBtn = GetComponent<Button>();
        interactBtn.onClick.AddListener(OnClick);
        this.repay = null;
        gameObject.SetActive(false);
        if (quest is QuestDebtRepay repay)
        {
            this.repay = repay;
            rate = repay._QuestID == "q1001" ? 5 : 10;
            repayAmount = Mathf.Abs(repay._ClearNum[0]) / rate;
            gameObject.SetActive(true);
        }

        LanguageText btnText = GetComponentInChildren<LanguageText>(true);
        if(btnText != null)
        {
            btnText.ChangeLangauge(SettingManager.Instance.language, interactBtnKey);
        }
    }
}
