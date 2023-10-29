using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Spine.Unity;

public class SkipDoorBtn : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public SkeletonGraphic img;
    private bool clicked = false;

    public void ResetAnimation()
    {
        if (!clicked)
            return;

        clicked = false;
        img.AnimationState.SetAnimation(1, "Fire", true);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (clicked)
            return;

        img.AnimationState.SetAnimation(1, "Basic_to_Over", false);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (clicked)
            return;

        img.AnimationState.SetAnimation(1, "Fire", true);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        clicked = true;
        img.AnimationState.SetAnimation(1, "Over_to_Open", false);
    }
}
