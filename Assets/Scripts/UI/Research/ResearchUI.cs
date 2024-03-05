using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResearchUI : PopUIControl
{
    private ResearchSlot curSelectedSlot = null;

    [SerializeField]
    private ResearchPopup researchPopup;

    public ResearchPopup _ResearchPopup { get => researchPopup; }

    public override void ResetPopUp()
    {
        SetClickedSlot(null);
        base.ResetPopUp();
    }

    public void SetClickedSlot(ResearchSlot researchSlot)
    {
        curSelectedSlot?.DeActiveClick();
        curSelectedSlot = researchSlot;
    }
}
