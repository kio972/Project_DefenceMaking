using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResearchMainUI : MonoBehaviour
{
    [SerializeField]
    GameObject uiPage;

    private ResearchSlot curResearch;

    private float curProgressTime;

    public float CurProgressTime { get => curProgressTime; }

    public float CurProgressRate { get => curResearch == null ? -1 : 1 - (curProgressTime / curResearch._ResearchData.requiredTime); }

    private Coroutine researchCoroutine;

    private List<string> completedResearchs;

    private IEnumerator IResearch(ResearchSlot curResearch, float additionalTime)
    {
        this.curResearch = curResearch;

        float startTime = (GameManager.Instance.CurWave * 1440) + GameManager.Instance.Timer;
        float endTime = startTime + curResearch._ResearchData.requiredTime - additionalTime;
        curResearch.SetResearchState(ResearchState.InProgress);
        yield return null;
        
        while(true)
        {
            float curTime = (GameManager.Instance.CurWave * 1440) + GameManager.Instance.Timer;
            if(curTime >= endTime)
                break;
            curProgressTime = endTime - curTime;
            yield return null;
        }

        curProgressTime = 0f;
        curResearch.SetResearchState(ResearchState.Complete);
        //ResearchPopup popup = GetComponentInChildren<ResearchPopup>();
        //if (popup.gameObject.activeSelf && popup._CurResearch == curResearch._ResearchData)
        //    curResearch.CallPopUpUI();
        GameManager.Instance.notificationBar?.SetMesseage(curResearch._ResearchData.researchName + " 연구 완료", NotificationType.Research);
        Research[] research = curResearch.GetComponents<Research>();
        foreach(var item in research)
            item?.ActiveResearch();
        completedResearchs.Add(curResearch._ResearchId);
        this.curResearch = null;
    }

    public bool StartResearch(ResearchSlot target, float additionalTime = 0)
    {
        if (curResearch != null)
            return false;

        researchCoroutine = StartCoroutine(IResearch(target, additionalTime));
        return true;
    }

    public void StopResearch(ResearchSlot target)
    {
        if (curResearch == null)
            return;

        StopCoroutine(researchCoroutine);
        curProgressTime = 0f;
        curResearch.SetResearchState(ResearchState.Incomplete);
        this.curResearch = null;
    }

    public void SetActive(bool value)
    {
        if (value)
            InputManager.Instance.ResetTileClick();

        UIManager.Instance.SetTab(uiPage, value, () => { GameManager.Instance.SetPause(false); });
        GameManager.Instance.SetPause(value);
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            if (UIManager.Instance._OpendUICount == 0 && !GameManager.Instance.isPause)
                SetActive(true);
            else if (uiPage.activeSelf)
                SetActive(false);
        }
    }

    public void SaveData(PlayerData data)
    {
        data.researchIdes = new List<string>(completedResearchs);
        if(curResearch != null)
        {
            data.curResearch = curResearch._ResearchId;
            data.curResearchTime = curResearch._ResearchData.requiredTime - curProgressTime;
        }
    }

    public void LoadData(PlayerData data)
    {
        completedResearchs = new List<string>(data.researchIdes);

        ResearchSlot[] slots = GetComponentsInChildren<ResearchSlot>(true);
        foreach (ResearchSlot slot in slots)
        {
            if (completedResearchs.Contains(slot._ResearchId))
            {
                slot.SetResearchState(ResearchState.Complete);
                Research research = slot.GetComponent<Research>();
                research?.ActiveResearch();
            }
            else if(!string.IsNullOrEmpty(data.curResearch) && slot._ResearchId == data.curResearch)
                StartResearch(slot, data.curResearchTime);
        }
    }

    private void Awake()
    {
        if(completedResearchs == null)
        {
            completedResearchs = new List<string>() { "r_m10001" };
            ResearchSlot[] slots = GetComponentsInChildren<ResearchSlot>(true);
            foreach (ResearchSlot slot in slots)
            {
                if (!completedResearchs.Contains(slot._ResearchId))
                    continue;

                slot.SetResearchState(ResearchState.Complete);
                Research research = slot.GetComponent<Research>();
                research?.ActiveResearch();
            }
        }
    }
}
