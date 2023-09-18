using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class MouseOverEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    private TextMeshProUGUI text;

    private TextMeshProUGUI _text
    {
        get
        {
            if (text == null)
                text = GetComponent<TextMeshProUGUI>();
            return text;
        }
    }

    private string originText;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_text == null)
            return;

        originText = _text.text;
        _text.text = "¡Ø " + originText + " ¡Ø";
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (_text == null)
            return;

        if (originText == null)
            originText = _text.text;

        _text.text = originText;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        OnPointerExit(eventData);
    }
}
