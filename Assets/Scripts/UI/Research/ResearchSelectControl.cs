using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResearchSelectControl : MonoBehaviour
{
    ResearchSelectBtn curBtn = null;

    ResearchSelectBtn[] _researchBtns;
    ResearchSelectBtn[] researchBtns { get => _researchBtns; }

    [SerializeField]
    private ResearchPopup popUp;

    public void SetCurResearch(ResearchSelectBtn target)
    {
        curBtn?.DeActiveClick();
        curBtn = target;
        popUp?.ResetPopUp();
    }

    private void Awake()
    {
        _researchBtns = GetComponentsInChildren<ResearchSelectBtn>();
        if (_researchBtns.Length == 0)
            return;
        foreach(ResearchSelectBtn btn in _researchBtns)
        {
            btn.Init();
            btn.DeActiveClick();
        }

        _researchBtns[0].OnClick();
    }
}
