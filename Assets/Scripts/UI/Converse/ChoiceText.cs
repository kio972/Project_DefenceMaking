using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ChoiceText : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private TextMeshProUGUI text;

    private char choice;

    private Button btn;

    public void OnPointerEnter(PointerEventData eventData)
    {
        text.transform.localScale = Vector3.one * 1.1f;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        text.transform.localScale = Vector3.one;
    }

    private void MakeDecision()
    {
        ScriptManager.Instance.MakeChoice(choice);
        OnPointerExit(null);
    }

    public void SetChoice(string script, char choice)
    {
        if(text == null)
            text = GetComponentInChildren<TextMeshProUGUI>(true);
        text.text = "¡Ø " + script + " ¡Ø";
        this.choice = choice;
    }

    private void Awake()
    {
        if(btn == null)
            btn = GetComponentInChildren<Button>(true);
        btn.onClick.AddListener(MakeDecision);
    }
}
