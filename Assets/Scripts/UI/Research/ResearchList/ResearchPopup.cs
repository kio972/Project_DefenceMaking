using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ResearchPopup : MonoBehaviour
{
    [SerializeField]
    private Image icon;
    [SerializeField]
    private GameObject inprogressFrame;
    [SerializeField]
    private TextMeshProUGUI researchName;
    [SerializeField]
    private TextMeshProUGUI researchDesc;
    [SerializeField]
    private TextMeshProUGUI researchTime;
    [SerializeField]
    private TextMeshProUGUI herb1;
    [SerializeField]
    private TextMeshProUGUI herb2;
    [SerializeField]
    private TextMeshProUGUI herb3;
    [SerializeField]
    private TextMeshProUGUI gold;
    [SerializeField]
    private Button researchStartBtn;

    [SerializeField]
    private GameObject inProgressBtn;

    [SerializeField]
    private GameObject researchPart;
    [SerializeField]
    private GameObject completePart;

    private ResearchMainUI researchMain;

    private ResearchMainUI ResearchMain
    {
        get
        {
            if (researchMain == null)
                researchMain = GetComponentInParent<ResearchMainUI>();

            return researchMain;
        }
    }

    private ResearchSlot curResearch;

    private float CurTime
    {
        get
        {
            if (ResearchMain == null)
                return 0;

            return ResearchMain.CurProgressTime;
        }
    }

    bool haveGold; 
    bool haveHerb1;
    bool haveHerb2;
    bool haveHerb3;

    private void OnDisable()
    {
        this.gameObject.SetActive(false);
    }

    private string GetMinSecTime(float time)
    {
        return ((int)(time / 60)).ToString("00") + ":" + ((int)(time % 60)).ToString("00");
    }

    public void SetPopUp(ResearchSlot researchSlot, Sprite iconSprite, ResearchState curState)
    {
        curResearch = researchSlot;
        ResearchData researchData = researchSlot._ResearchData;

        icon.sprite = iconSprite;
        bool isInProgress = curState == ResearchState.InProgress;
        inprogressFrame.SetActive(isInProgress);
        researchTime.gameObject.SetActive(!(curState == ResearchState.Complete));
        researchStartBtn.gameObject.SetActive(curState == ResearchState.Incomplete);
        inProgressBtn.SetActive(curState == ResearchState.InProgress);
        if (isInProgress)
            researchTime.text = GetMinSecTime(CurTime);
        else
            researchTime.text = GetMinSecTime(researchData.requiredTime);

        researchName.text = researchData.researchName;
        researchDesc.text = researchData.researchDesc;

        herb1.text = researchData.requiredherb1.ToString();
        herb1.transform.parent.gameObject.SetActive(researchData.requiredherb1 != 0);

        herb2.text = researchData.requiredherb2.ToString();
        herb2.transform.parent.gameObject.SetActive(researchData.requiredherb2 != 0);

        herb3.text = researchData.requiredherb3.ToString();
        herb3.transform.parent.gameObject.SetActive(researchData.requiredherb3 != 0);

        gold.text = researchData.requiredMoney.ToString();
        gold.transform.parent.gameObject.SetActive(researchData.requiredMoney != 0);

        haveGold = GameManager.Instance.gold >= curResearch._ResearchData.requiredMoney;
        gold.color = haveGold ? Color.white : Color.red;

        haveHerb1 = GameManager.Instance.herb1 >= curResearch._ResearchData.requiredherb1;
        herb1.color = haveHerb1 ? Color.white : Color.red;

        haveHerb2 = GameManager.Instance.herb2 >= curResearch._ResearchData.requiredherb2;
        herb2.color = haveHerb2 ? Color.white : Color.red;

        haveHerb3 = GameManager.Instance.herb3 >= curResearch._ResearchData.requiredherb3;
        herb3.color = haveHerb3 ? Color.white : Color.red;

        researchPart.SetActive(curState != ResearchState.Complete);
        completePart.SetActive(curState == ResearchState.Complete);
    }

    private void ModifyAssets()
    {
        GameManager.Instance.gold -= curResearch._ResearchData.requiredMoney;
        GameManager.Instance.herb1 -= curResearch._ResearchData.requiredherb1;
        GameManager.Instance.herb2 -= curResearch._ResearchData.requiredherb2;
        GameManager.Instance.herb3 -= curResearch._ResearchData.requiredherb3;
    }

    public void StartResearch()
    {
        if (!(haveGold && haveHerb1 && haveHerb2 && haveHerb3))
            return;

        bool isStart = ResearchMain.StartResearch(curResearch);
        if(isStart)
        {
            inprogressFrame.SetActive(true);
            inProgressBtn.SetActive(true);
            ModifyAssets();
        }
    }
}
