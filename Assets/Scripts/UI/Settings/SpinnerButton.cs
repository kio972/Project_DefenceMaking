using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public abstract class SpinnerButton : MonoBehaviour
{
    [SerializeField]
    private List<string> values;

    [SerializeField]
    private Button leftBtn;

    [SerializeField]
    private Button rightBtn;

    [SerializeField]
    protected TextMeshProUGUI text;

    protected int index = 0;

    private void LeftBtn()
    {
        if (values == null)
            return;

        index--;
        if (index < 0)
            index = values.Count - 1;

        OnValueChange();
    }

    private void RightBtn()
    {
        if (values == null)
            return;

        index++;
        if (index >= values.Count)
            index = 0;

        OnValueChange();
    }

    private void Awake()
    {
        Init();
    }

    protected virtual void Init()
    {
        if (leftBtn != null)
            leftBtn.onClick.AddListener(LeftBtn);
        if (rightBtn != null)
            rightBtn.onClick.AddListener(RightBtn);
        
    }

    protected virtual void OnValueChange()
    {
        if (text == null)
            return;

        text.text = values[index];
        LanguageText languageText = text.GetComponent<LanguageText>();
        languageText?.ChangeLangauge(SettingManager.Instance.language, values[index]);
    }
}