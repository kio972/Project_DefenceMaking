using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MouseOverIcon : MouseOverTarget
{
    [SerializeField]
    private string headerKey;
    [SerializeField]
    private string descKey;
    [SerializeField]
    private string additionalKey;

    [SerializeField]
    private bool isOnUI = true;

    [SerializeField]
    private Vector2 pivot = new Vector2(0.5f, 0.5f);

    public override void SetText()
    {
        string header = DataManager.Instance.GetDescription(headerKey);
        string desc = DataManager.Instance.GetDescription(descKey);
        string additional = DataManager.Instance.GetDescription(additionalKey);
        GameManager.Instance._InGameUI.mouseOverTooltip?.SetMesseage(transform, pivot, isOnUI, header, desc, additional);
    }
}
