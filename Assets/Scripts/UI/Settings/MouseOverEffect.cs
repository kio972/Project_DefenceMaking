using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using Spine;

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

    [SerializeField]
    private Transform mouseOverEffect;

    private Transform _mouseOverEffect
    {
        get
        {
            if (mouseOverEffect == null && transform.childCount > 0)
                mouseOverEffect = transform.GetChild(0);
            return mouseOverEffect;
        }
    }

    private void OnDisable()
    {
        OnPointerExit(null);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_mouseOverEffect == null)
            return;

        _mouseOverEffect.gameObject.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (_mouseOverEffect == null)
            return;

        _mouseOverEffect.gameObject.SetActive(false);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        OnPointerExit(eventData);
    }
}
