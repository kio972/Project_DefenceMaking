using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(TextMeshProUGUI))]
public class LanguageText : MonoBehaviour
{
    [SerializeField]
    protected string keyStr;
    public string KeyStr { get => keyStr; }

    private TextMeshProUGUI text;

    private float originSize;

    public TextMeshProUGUI _Text
    {
        get
        {
            if (text == null)
            {
                text = GetComponent<TextMeshProUGUI>();
                originSize = text.fontSize;
            }
            return text;
        }
    }

    private void Awake()
    {
        if (text == null)
        {
            text = GetComponent<TextMeshProUGUI>();
            originSize = text.fontSize;
        }
    }

    public string DisplayText { get { return keyStr; } }

    public void ChangeTextSize(float mult)
    {
        float temp = _Text.fontSize;
        mult = Mathf.Clamp(mult, 0.8f, 1.2f);
        float targetSize = Mathf.Round(originSize * mult);
        _Text.fontSize = targetSize;
    }

    public void ChangeLangauge(Languages language)
    {
        if (string.IsNullOrEmpty(keyStr))
            return;

        // 1. text의 key값으로 인덱스 불러오기
        int index = UtilHelper.Find_Data_Index(keyStr, DataManager.Instance.Language_Table, "id");
        if (index == -1)
        {
            _Text.text = keyStr;
            return;
        }

        string targetLanguage = language.ToString();
        Dictionary<string, object> data = DataManager.Instance.Language_Table[index];
        if(!data.ContainsKey(targetLanguage))
        {
            _Text.text = keyStr;
            return;
        }

        _Text.text = data[targetLanguage].ToString();
    }
}
