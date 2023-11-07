using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;

public class MouseOverEffect2 : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    private Image img;
    private bool isMouseOver;
    public bool IsMouseOver { get => isMouseOver; }

    private Sprite originSprite;
    [SerializeField]
    private Sprite changeSprite;

    private Image _img
    {
        get
        {
            if (img == null)
            {
                img = GetComponent<Image>();
                originSprite = img.sprite;
            }
            return img;
        }
    }

    private string originText;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_img == null)
            return;
        isMouseOver = true;
        if(changeSprite != null)
            _img.sprite = changeSprite;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (_img == null)
            return;
        isMouseOver = false;
        _img.sprite = originSprite;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        OnPointerExit(eventData);
    }
}
