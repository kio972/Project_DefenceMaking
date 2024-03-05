using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResearchSelectControl : MonoBehaviour
{
    ResearchSelectBtn curBtn = null;
    [SerializeField]
    GameObject popUpUI;

    public void SetCurResearch(ResearchSelectBtn target)
    {
        curBtn?.DeActiveClick();
        curBtn = target;
    }

    private void Awake()
    {
        ResearchSelectBtn[] btns = GetComponentsInChildren<ResearchSelectBtn>();
        if (btns.Length == 0)
            return;
        foreach(ResearchSelectBtn btn in btns)
        {
            btn.Init();
            btn.DeActiveClick();
        }

        btns[0].OnClick();
    }
}
