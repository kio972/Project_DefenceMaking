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
    private TextMeshProUGUI hurb1;
    [SerializeField]
    private TextMeshProUGUI hurb2;
    [SerializeField]
    private TextMeshProUGUI hurb3;
    [SerializeField]
    private TextMeshProUGUI gold;
    [SerializeField]
    private Button researchStartBtn;

    private ResearchMainUI researchMain;
    private float CurTime
    {
        get
        {
            if (researchMain == null)
                researchMain = GetComponentInParent<ResearchMainUI>();
            if (researchMain == null)
                return 0;

            return researchMain.CurProgressTime;
        }
    }

    private string GetMinSecTime(float time)
    {
        return ((int)(time / 60)).ToString("00") + ":" + ((int)(time % 60)).ToString("00");
    }

    public void SetPopUp(ResearchData researchData, Sprite iconSprite, ResearchState curState)
    {
        icon.sprite = iconSprite;
        bool isInProgress = curState == ResearchState.InProgress;
        inprogressFrame.SetActive(isInProgress);
        researchTime.gameObject.SetActive(!(curState == ResearchState.Complete));
        researchStartBtn.gameObject.SetActive(curState == ResearchState.Incomplete);
        if (isInProgress)
            researchTime.text = GetMinSecTime(CurTime);
        else
            researchTime.text = GetMinSecTime(researchData.requiredTime);

        researchName.text = researchData.researchName;
        researchDesc.text = researchData.researchDesc;

        hurb1.text = researchData.requiredHurb1.ToString();
        hurb1.transform.parent.gameObject.SetActive(researchData.requiredHurb1 != 0);

        hurb2.text = researchData.requiredHurb2.ToString();
        hurb2.transform.parent.gameObject.SetActive(researchData.requiredHurb2 != 0);

        hurb3.text = researchData.requiredHurb3.ToString();
        hurb3.transform.parent.gameObject.SetActive(researchData.requiredHurb3 != 0);

        gold.text = researchData.requiredMoney.ToString();
        gold.transform.parent.gameObject.SetActive(researchData.requiredMoney != 0);
    }
}
