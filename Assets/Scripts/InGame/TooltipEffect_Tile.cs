using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TooltipEffect_Tile : MonoBehaviour, IToolTipEffect
{
    Tile tile;
    public void ShowEffect(bool value)
    {
        if(tile == null)
            tile = GetComponentInParent<Tile>();

        if(value)
            NodeManager.Instance.SetGuideState(GuideState.Selected, tile);
        else
            NodeManager.Instance.SetGuideState(GuideState.None);
    }
}
