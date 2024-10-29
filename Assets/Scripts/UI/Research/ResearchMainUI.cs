using Spine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ResearchMainUI : MonoBehaviour
{
    [SerializeField]
    GameObject uiPage;

    public ResearchSlot curResearch { get; private set; }

    private float curProgressTime;

    public float CurProgressTime { get => curProgressTime; }

    public float CurProgressRate { get => curResearch == null ? -1 : 1 - (curProgressTime / curResearch._ResearchData.requiredTime); }

    private Coroutine researchCoroutine;

    private List<string> _completedResearchs;
    public List<string> completedResearchs { get => new List<string>(_completedResearchs); }

    private IEnumerator IResearch(ResearchSlot curResearch, float additionalTime)
    {
        this.curResearch = curResearch;

        float startTime = (GameManager.Instance.CurWave * 1440) + GameManager.Instance.Timer;
        float endTime = startTime + curResearch._ResearchData.requiredTime - additionalTime;
        curResearch.SetResearchState(ResearchState.InProgress);
        float curTime = startTime;
        yield return null;
        
        while(true)
        {
            curTime += GameManager.Instance.InGameDeltaTime;
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
        AudioManager.Instance.Play2DSound("Complete_Tech", SettingManager.Instance._FxVolume);

        this.curResearch = null;
        _completedResearchs.Add(curResearch._ResearchId);
        Research[] research = curResearch.GetComponents<Research>();
        foreach(var item in research)
            item?.ActiveResearch();
        
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
        {
            InputManager.Instance.ResetTileClick();
            AudioManager.Instance.Play2DSound("Tech_research_Open", SettingManager.Instance._FxVolume);
            GameManager.Instance.SetPause(true);
        }

        UIManager.Instance.SetTab(uiPage, value, () => { GameManager.Instance.SetPause(false); });
    }

    [SerializeField]
    private GameObject btnObject;

    public void Update()
    {
        if (Input.GetKeyDown(SettingManager.Instance.key_Research._CurKey) && btnObject.activeSelf)
        {
            if (UIManager.Instance._OpendUICount == 0 && !GameManager.Instance.isPause)
                SetActive(true);
            else if (uiPage.activeSelf)
                SetActive(false);
        }
    }

    public void SaveData(PlayerData data)
    {
        data.researchIdes = new List<string>(_completedResearchs);
        if(curResearch != null)
        {
            data.curResearch = curResearch._ResearchId;
            data.curResearchTime = curResearch._ResearchData.requiredTime - curProgressTime;
        }
    }

    public void LoadData(PlayerData data)
    {
        _completedResearchs = new List<string>(data.researchIdes);

        ResearchSlot[] slots = GetComponentsInChildren<ResearchSlot>(true);
        foreach (ResearchSlot slot in slots)
        {
            if (_completedResearchs.Contains(slot._ResearchId))
            {
                slot.SetResearchState(ResearchState.Complete);
                Research research = slot.GetComponent<Research>();
                research?.ActiveResearch();
            }
            else if(!string.IsNullOrEmpty(data.curResearch) && slot._ResearchId == data.curResearch)
                StartResearch(slot, data.curResearchTime);
        }
    }

    public void ForceActiveResearch(string id)
    {
        ResearchSlot slot = researchDic[id];
        slot.SetResearchState(ResearchState.Complete);
        Research research = slot.GetComponent<Research>();
        research?.ActiveResearch();
    }

    private Dictionary<string, ResearchSlot> researchDic = new Dictionary<string, ResearchSlot>();

    private void Awake()
    {
        if(_completedResearchs == null)
        {
            _completedResearchs = new List<string>();
            ResearchSlot[] slots = GetComponentsInChildren<ResearchSlot>(true);
            foreach (ResearchSlot slot in slots)
            {
                if (string.IsNullOrEmpty(slot._ResearchId) || researchDic.ContainsKey(slot._ResearchId))
                    continue;

                researchDic.Add(slot._ResearchId, slot);
                if (!_completedResearchs.Contains(slot._ResearchId))
                    continue;

                slot.SetResearchState(ResearchState.Complete);
                Research research = slot.GetComponent<Research>();
                research?.ActiveResearch();
            }
        }
    }
}
