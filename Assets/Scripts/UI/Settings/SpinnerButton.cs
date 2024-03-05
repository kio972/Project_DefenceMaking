using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum SpineType
{
    Repeat,
    Clamp,
}

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

    [SerializeField]
    protected SpineType spineType = SpineType.Repeat;

    private void LeftBtn()
    {
        if (values == null)
            return;

        index--;
        

        if(spineType == SpineType.Repeat && index < 0)
            index = values.Count - 1;
        if(spineType == SpineType.Clamp)
            SetBtn();

        OnValueChange();
    }

    private void RightBtn()
    {
        if (values == null)
            return;

        index++;
        if (spineType == SpineType.Repeat && index >= values.Count)
            index = 0;
        if (spineType == SpineType.Clamp)
            SetBtn();


        OnValueChange();
    }

    protected void SetBtn()
    {
        if (spineType != SpineType.Clamp)
            return;

        leftBtn.gameObject.SetActive(index != 0);
        rightBtn.gameObject.SetActive(index != values.Count - 1);
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