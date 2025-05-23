using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BribeBtn : MouseOverTarget
{
    private Adventurer battler;
    private int cost;

    [SerializeField]
    FMODUnity.EventReference successClip;
    [SerializeField]
    FMODUnity.EventReference failClip;
    readonly Vector2 pivot = new Vector2(0.5f, -0.2f);

    public override void SetText()
    {
        string header = DataManager.Instance.GetDescription("shop_item0021_0");
        string desc = DataManager.Instance.GetDescription("tooltip_executor1_1");
        string additional = $"{DataManager.Instance.GetDescription("ui_GoldCost")} : <color=yellow>{cost}</color> <sprite name=\"Gold\">";
        GameManager.Instance._InGameUI.mouseOverTooltip?.SetMesseage(transform, pivot, false, header, desc, additional);
    }

    public void ExcuteBribe()
    {
        if (cost > GameManager.Instance.gold)
        {
            FMODUnity.RuntimeManager.PlayOneShot(failClip);
            return;
        }

        GameManager.Instance.gold -= cost;
        FMODUnity.RuntimeManager.PlayOneShot(successClip);
        battler.ReturnToBase(false);
    }

    protected override void OnDisable()
    {
        gameObject.SetActive(false);
        base.OnDisable();
    }

    public void Init(Adventurer target, int cost)
    {
        this.cost = cost;
        battler = target;
        gameObject.SetActive(true);
    }
}
