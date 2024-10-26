using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TooltipEffect_Image : MonoBehaviour, IToolTipEffect
{
    [SerializeField]
    Outline targetOutline;
    public void ShowEffect(bool value)
    {
        if (targetOutline == null)
            return;

        targetOutline.enabled = value;
    }
}
