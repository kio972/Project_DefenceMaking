using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

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
    private TextMeshProUGUI researchTimer;
    [SerializeField]
    private TextMeshProUGUI herb1;
    [SerializeField]
    private TextMeshProUGUI herb2;
    [SerializeField]
    private TextMeshProUGUI herb3;
    [SerializeField]
    private TextMeshProUGUI gold;

    [SerializeField]
    private GameObject researchStartBtn;
    [SerializeField]
    private GameObject inProgressBtn;
    [SerializeField]
    private GameObject researchStopBtn;

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

    [SerializeField]
    ResearchState researchState;
    bool haveGold; 
    bool haveHerb1;
    bool haveHerb2;
    bool haveHerb3;

    //private void OnDisable()
    //{
    //    this.gameObject.SetActive(false);
    //}

    private string GetMinSecTime(float time)
    {
        return ((int)(time / 60)).ToString("00") + ":" + ((int)(time % 60)).ToString("00");
    }

    public void SetPopUp(ResearchSlot researchSlot, Sprite iconSprite, ResearchState curState)
    {
        curResearch = researchSlot;
        ResearchData researchData = researchSlot._ResearchData;
        this.researchState = curState;

        icon.sprite = iconSprite;
        bool isInProgress = curState == ResearchState.InProgress;
        inprogressFrame.SetActive(isInProgress);
        researchTimer.gameObject.SetActive(isInProgress);
        if (isInProgress)
            researchTimer.text = GetMinSecTime(CurTime);

        researchName.text = researchData.researchName;
        researchDesc.text = researchData.researchDesc;
        researchTime.text = GetMinSecTime(researchData.requiredTime);

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

        SetResearchBtn(researchState);
    }

    private bool HaveAsset()
    {
        return haveGold && haveHerb1 && haveHerb2 && haveHerb3;
    }

    private void ModifyAssets(bool isPlus)
    {
        int plusMinus = isPlus ? 1 : -1;

        GameManager.Instance.gold += curResearch._ResearchData.requiredMoney * plusMinus;
        GameManager.Instance.herb1 += curResearch._ResearchData.requiredherb1 * plusMinus;
        GameManager.Instance.herb2 += curResearch._ResearchData.requiredherb2 * plusMinus;
        GameManager.Instance.herb3 += curResearch._ResearchData.requiredherb3 * plusMinus;
    }

    private void SetResearchBtn(ResearchState curState)
    {
        researchStartBtn?.SetActive(false);
        inProgressBtn?.SetActive(false);
        researchStopBtn?.SetActive(false);

        switch(curState)
        {
            case ResearchState.InProgress:
                researchStopBtn?.SetActive(true);
                break;
            case ResearchState.Incomplete:
                researchStartBtn?.SetActive(HaveAsset());
                inProgressBtn?.SetActive(!HaveAsset());
                break;
        }

        researchPart.SetActive(curState != ResearchState.Complete);
        completePart.SetActive(curState == ResearchState.Complete);
    }

    public void ResearchInteract(bool blockStop = false)
    {
        ResearchState curState = researchState;
        if (curState == ResearchState.Incomplete)
            StartResearch();
        else if (curState == ResearchState.InProgress && !blockStop)
            StopResearch();

        if(curState != researchState)
            SetResearchBtn(researchState);
    }

    private void StartResearch()
    {
        if (!HaveAsset())
        {
            GameManager.Instance.popUpMessage.ToastMsg("연구 재화가 부족합니다.");
            AudioManager.Instance.Play2DSound("UI_Click_DownPitch_01", SettingManager.Instance._UIVolume);
            return;
        }

        bool isStart = ResearchMain.StartResearch(curResearch);
        if (isStart)
        {
            inprogressFrame.SetActive(true);
            inProgressBtn.SetActive(true);
            researchTimer.gameObject.SetActive(true);
            researchTimer.text = GetMinSecTime(curResearch._ResearchData.requiredTime);
            ModifyAssets(false);
            researchState = ResearchState.InProgress;
        }
        else
            GameManager.Instance.popUpMessage.ToastMsg("연구가 이미 진행중입니다.");

        AudioManager.Instance.Play2DSound(isStart ? "UI_Click_01" : "UI_Click_DownPitch_01", SettingManager.Instance._UIVolume);
    }

    private void StopResearch()
    {
        ResearchMain.StopResearch(curResearch);
        inprogressFrame.SetActive(false);
        inProgressBtn.SetActive(false);
        researchTimer.gameObject.SetActive(false);
        ModifyAssets(true);
        researchState = ResearchState.Incomplete;

        AudioManager.Instance.Play2DSound("UI_Click_DownPitch_01", SettingManager.Instance._UIVolume);
    }
}
