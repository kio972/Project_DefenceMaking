using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

public class ResearchPopup : MonoBehaviour, ISlotInformer
{
    [SerializeField]
    private Image icon;
    [SerializeField]
    private GameObject inprogressFrame;
    [SerializeField]
    private LanguageText researchName;
    [SerializeField]
    private LanguageText researchDesc;
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

    [SerializeField]
    private GameObject researchInfos;

    private ResearchMainUI _researchMain;

    private ResearchMainUI researchMain
    {
        get
        {
            if (_researchMain == null)
                _researchMain = GetComponentInParent<ResearchMainUI>();

            return _researchMain;
        }
    }

    private ResearchSlot curResearch;

    private float CurTime
    {
        get
        {
            if (researchMain == null)
                return 0;

            return researchMain.CurProgressTime;
        }
    }

    [SerializeField]
    ResearchState researchState;
    bool haveGold; 
    bool haveHerb1;
    bool haveHerb2;
    bool haveHerb3;

    public ISlot curSlot { get => curResearch; }
    public void ExcuteAction()
    {

    }

    void Start()
    {
        ResetPopUp();
    }

    private string GetMinSecTime(float time)
    {
        return ((int)(time / 60)).ToString("00") + ":" + ((int)(time % 60)).ToString("00");
    }

    public void ResetPopUp()
    {
        curResearch = null;
        researchInfos.SetActive(false);
    }

    private void OnEnable()
    {
        if(curResearch != null)
            SetPopUp(curResearch, icon.sprite, curResearch._CurState);
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

        researchName.ChangeLangauge(SettingManager.Instance.language, researchData.researchName);
        researchDesc.ChangeLangauge(SettingManager.Instance.language, researchData.researchDesc);
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

        haveHerb1 = GameManager.Instance.herbDic[HerbType.BlackHerb] >= curResearch._ResearchData.requiredherb1;
        herb1.color = haveHerb1 ? Color.white : Color.red;

        haveHerb2 = GameManager.Instance.herbDic[HerbType.PurpleHerb] >= curResearch._ResearchData.requiredherb2;
        herb2.color = haveHerb2 ? Color.white : Color.red;

        haveHerb3 = GameManager.Instance.herbDic[HerbType.WhiteHerb] >= curResearch._ResearchData.requiredherb3;
        herb3.color = haveHerb3 ? Color.white : Color.red;

        researchInfos.SetActive(true);
        SetResearchBtn(researchSlot.IsLock ? ResearchState.Impossible : researchState);
    }

    private bool HaveAsset()
    {
        return haveGold && haveHerb1 && haveHerb2 && haveHerb3;
    }

    private void ModifyAssets(bool isPlus)
    {
        int plusMinus = isPlus ? 1 : -1;

        GameManager.Instance.gold += curResearch._ResearchData.requiredMoney * plusMinus;
        GameManager.Instance.herbDic[HerbType.BlackHerb] += curResearch._ResearchData.requiredherb1 * plusMinus;
        GameManager.Instance.herbDic[HerbType.PurpleHerb] += curResearch._ResearchData.requiredherb2 * plusMinus;
        GameManager.Instance.herbDic[HerbType.WhiteHerb] += curResearch._ResearchData.requiredherb3 * plusMinus;
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
        if (researchMain.curResearch != null && researchMain.curResearch != curResearch)
            GameManager.Instance.popUpMessage.ToastMsg(DataManager.Instance.GetDescription("announce_ingame_requireNoResearch"));
        else if (curState == ResearchState.Incomplete)
            StartResearch();
        else if (curState == ResearchState.InProgress && !blockStop)
            StopResearch();

        if (curState != researchState)
            SetResearchBtn(researchState);
    }

    [SerializeField]
    FMODUnity.EventReference refusedSound;
    [SerializeField]
    FMODUnity.EventReference excutedSound;

    private void StartResearch()
    {
        if (!HaveAsset())
        {
            GameManager.Instance.popUpMessage.ToastMsg(DataManager.Instance.GetDescription("announce_ingame_requireAsset"));
            //AudioManager.Instance.Play2DSound("UI_Click_DownPitch_01", SettingManager.Instance._UIVolume);
            FMODUnity.RuntimeManager.PlayOneShot(refusedSound);
            return;
        }

        bool isStart = researchMain.StartResearch(curResearch);
        if (isStart)
        {
            inprogressFrame.SetActive(true);
            inProgressBtn.SetActive(true);
            researchTimer.gameObject.SetActive(true);
            researchTimer.text = GetMinSecTime(curResearch._ResearchData.requiredTime);
            ModifyAssets(false);
            researchState = ResearchState.InProgress;
        }

        //AudioManager.Instance.Play2DSound(isStart ? "UI_Click_01" : "UI_Click_DownPitch_01", SettingManager.Instance._UIVolume);
        if(isStart)
            FMODUnity.RuntimeManager.PlayOneShot(excutedSound);
        else
            FMODUnity.RuntimeManager.PlayOneShot(refusedSound);
    }

    private void StopResearch()
    {
        researchMain.StopResearch(curResearch);
        inprogressFrame.SetActive(false);
        inProgressBtn.SetActive(false);
        researchTimer.gameObject.SetActive(false);
        ModifyAssets(true);
        researchState = ResearchState.Incomplete;

        //AudioManager.Instance.Play2DSound("UI_Click_DownPitch_01", SettingManager.Instance._UIVolume);
        FMODUnity.RuntimeManager.PlayOneShot(refusedSound);
    }
}
