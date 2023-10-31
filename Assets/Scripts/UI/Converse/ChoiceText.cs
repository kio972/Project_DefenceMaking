using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChoiceText : MonoBehaviour
{
    private TextMeshProUGUI text;

    private char choice;

    private Button btn;

    
    private void MakeDecision()
    {
        ScriptManager.Instance.MakeChoice(choice);
    }

    public void SetChoice(string script, char choice)
    {
        if(text == null)
            text = GetComponentInChildren<TextMeshProUGUI>(true);
        text.text = script;
        this.choice = choice;
    }

    private void Awake()
    {
        if(btn == null)
            btn = GetComponentInChildren<Button>(true);
        btn.onClick.AddListener(MakeDecision);
    }
}
