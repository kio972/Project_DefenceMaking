using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResearchSelectControl : MonoBehaviour
{
    ResearchSelectBtn _curBtn = null;

    ResearchSelectBtn[] _researchBtns;
    public ResearchSelectBtn curBtn { get => _curBtn; }

    [SerializeField]
    private ResearchPopup popUp;

    public void SetCurResearch(ResearchSelectBtn target)
    {
        _curBtn?.DeActiveClick();
        _curBtn = target;
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
