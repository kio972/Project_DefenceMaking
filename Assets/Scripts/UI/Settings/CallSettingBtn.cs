using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class CallSettingBtn : MonoBehaviour
{
    private Button btn;
    public bool value = true;

    private void CallSetting()
    {
        SettingCanvas.Instance.CallSettings(value);
        SettingManager.Instance.SetLanguage();
    }

    private void Awake()
    {
        btn = GetComponent<Button>();
        if (btn != null)
            btn.onClick.AddListener(CallSetting);
    }
}
