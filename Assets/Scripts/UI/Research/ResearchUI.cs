using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResearchUI : PopUIControl
{
    private ResearchSlot curSelectedSlot = null;

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
