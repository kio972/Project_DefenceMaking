using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TooltipEffect_Character : MonoBehaviour, IToolTipEffect
{
    [SerializeField]
    GameObject targetOutline;
    public void ShowEffect(bool value)
    {
        if (targetOutline == null)
            return;

        targetOutline.SetActive(value);
    }
}
