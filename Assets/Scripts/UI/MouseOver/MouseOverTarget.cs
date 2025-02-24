using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class MouseOverTarget : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public void OnPointerExit(PointerEventData eventData)
    {
        GameManager.Instance._InGameUI.mouseOverTooltip?.SetActive(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        SetText();
        GameManager.Instance._InGameUI.mouseOverTooltip?.SetActive(true);
    }

    public abstract void SetText();

    private void OnDisable()
    {
        gameObject.SetActive(false);
        OnPointerExit(null);
    }
}
