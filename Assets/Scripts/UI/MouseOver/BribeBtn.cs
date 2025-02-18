using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class BribeBtn : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Adventurer battler;
    private int cost;

    [SerializeField]
    private AK.Wwise.Event successClip;
    [SerializeField]
    private AK.Wwise.Event failClip;
    readonly Vector2 pivot = new Vector2(0.5f, -0.2f);

    public void OnPointerExit(PointerEventData eventData)
    {
        GameManager.Instance._InGameUI.mouseOverTooltip?.SetActive(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        SetText();
        GameManager.Instance._InGameUI.mouseOverTooltip?.SetActive(true);
    }

    private void SetText()
    {
        string header = DataManager.Instance.GetDescription("shop_item0021_0");
        string desc = DataManager.Instance.GetDescription("tooltip_executor1_1");
        string additional = $"{DataManager.Instance.GetDescription("ui_GoldCost")} : <color=yellow>{cost}</color> <sprite name=\"Gold\">";
        GameManager.Instance._InGameUI.mouseOverTooltip?.SetMesseage(transform, pivot, header, desc, additional);
    }

    public void ExcuteBribe()
    {
        if (cost > GameManager.Instance.gold)
        {
            failClip?.Post(gameObject);
            return;
        }

        GameManager.Instance.gold -= cost;
        successClip?.Post(gameObject);
        battler.ReturnToBase(false);
    }

    public void Init(Adventurer target, int cost)
    {
        this.cost = cost;
        battler = target;
        gameObject.SetActive(true);
    }

    private void OnDisable()
    {
        gameObject.SetActive(false);
        OnPointerExit(null);
    }
}
