using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResearchSelectControl : MonoBehaviour
{
    ResearchSelectBtn curBtn = null;
    [SerializeField]
    GameObject popUpUI;

    ResearchSelectBtn[] _researchBtns;
    ResearchSelectBtn[] researchBtns { get => _researchBtns; }

    public void SetCurResearch(ResearchSelectBtn target)
    {
        curBtn?.DeActiveClick();
        curBtn = target;
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
