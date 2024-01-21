using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResearchUI : MonoBehaviour
{
    private ResearchSlot curSelectedSlot = null;

    public void SetClickedSlot(ResearchSlot researchSlot)
    {
        curSelectedSlot?.DeActiveClick();
        curSelectedSlot = researchSlot;
    }
}
