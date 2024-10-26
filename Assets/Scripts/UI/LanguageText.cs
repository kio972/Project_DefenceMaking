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

    [SerializeField]
    private bool sizeLock = false;

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
        if (sizeLock)
            return;

        float temp = _Text.fontSize;
        mult = Mathf.Clamp(mult, 0.8f, 1.2f);
        float targetSize = Mathf.Round(originSize * mult);
        _Text.fontSize = targetSize;
    }

    public void ChangeLangauge(Languages language, string key = null)
    {
        if (key != null)
            keyStr = key;
        if (string.IsNullOrEmpty(keyStr))
            return;

        // 1. text�� key������ �ε��� �ҷ�����
        int index = UtilHelper.Find_Data_Index(keyStr, DataManager.Instance.language_Table, "id");
        if (index == -1)
        {
            _Text.text = keyStr;
            return;
        }

        string targetLanguage = language.ToString();
        Dictionary<string, object> data = DataManager.Instance.language_Table[index];
        if(!data.ContainsKey(targetLanguage))
        {
            _Text.text = keyStr;
            return;
        }

        _Text.text = data[targetLanguage].ToString();
    }
}
