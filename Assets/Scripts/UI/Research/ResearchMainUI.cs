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

    private IEnumerator IResearch(ResearchSlot curResearch)
    {
        this.curResearch = curResearch;

        float startTime = (GameManager.Instance.CurWave * 1440) + GameManager.Instance.Timer;
        float endTime = startTime + curResearch._ResearchData.requiredTime;
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

        Research research = curResearch.GetComponent<Research>();
        research?.ActiveResearch();

        this.curResearch = null;
    }

    public bool StartResearch(ResearchSlot target)
    {
        if (curResearch != null)
            return false;

        StartCoroutine(IResearch(target));
        return true;
    }

    public void SetActive(bool value)
    {
        UIManager.Instance.SetTab(uiPage, value, () => { GameManager.Instance.SetPause(false); });
        GameManager.Instance.SetPause(value);
        //if (value)
        //    SetItem();
    }
}
