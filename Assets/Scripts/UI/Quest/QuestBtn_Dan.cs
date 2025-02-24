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

    readonly string successClipName = "Use_Coin";
    readonly string faillClipName = "UI_Fail";

    readonly Vector2 pivot = new Vector2(-0.2f, 0.5f);
    public void OnClick()
    {
        if (repay == null || repay._IsClear)
            return;

        if (repayAmount > GameManager.Instance.gold)
        {
            AkSoundEngine.PostEvent(faillClipName, gameObject);
            return;
        }

        repay.ReduceGold(repayAmount);
        AkSoundEngine.PostEvent(successClipName, gameObject);
        repay.UpdateQuest();
        if (repay._IsComplete[0])
            gameObject.SetActive(false);
    }

    public override void SetText()
    {
        string header = DataManager.Instance.GetDescription("shop_item0021_0");
        string desc = "";
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
    }
}
