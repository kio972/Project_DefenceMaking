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

    [SerializeField]
    private BuffInfo buffInfo;
    public bool setInfo = false;

    private string script;

    public void OnPointerEnter(PointerEventData eventData)
    {
        text.transform.localScale = Vector3.one * 1.1f;
        text.text = "�� " + script + " ��";
        text.color = Color.white;
        if (setInfo)
        {
            buffInfo.gameObject.SetActive(true);
            buffInfo.SetInfo(DataManager.Instance.BuffTable[GameManager.Instance.loop][choice]);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        text.transform.localScale = Vector3.one;
        text.text = script;
        text.color = Color.gray;
        if (setInfo)
        {
            buffInfo.gameObject.SetActive(false);
        }
    }

    private void MakeDecision()
    {
        StoryManager.Instance.MakeChoice(choice);
        OnPointerExit(null);
    }

    public void SetChoice(string script, char choice)
    {
        if(text == null)
            text = GetComponentInChildren<TextMeshProUGUI>(true);
        text.text = script;
        text.color = Color.gray;
        this.script = script;
        this.choice = choice;
    }

    private void Awake()
    {
        if(btn == null)
            btn = GetComponentInChildren<Button>(true);
        btn.onClick.AddListener(MakeDecision);
    }
}
