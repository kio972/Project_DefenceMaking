using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TooltipEffect_Tile : MonoBehaviour, IToolTipEffect
{
    ITileKind tile;
    public void ShowEffect(bool value)
    {
        if(tile == null)
            tile = GetComponentInParent<ITileKind>();

        if(value)
            NodeManager.Instance.SetGuideState(GuideState.Selected, tile);
        else
            NodeManager.Instance.SetGuideState(GuideState.None);
    }
}
