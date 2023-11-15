using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class CallSettingBtn : MonoBehaviour
{
    private Button btn;
    public bool value = true;
    public bool inIngame = false;

    private void CallSetting()
    {
        SettingCanvas.Instance.CallSettings(value, inIngame);
        SettingManager.Instance.SetLanguage();
    }

    private void Awake()
    {
        btn = GetComponent<Button>();
        if (btn != null)
            btn.onClick.AddListener(CallSetting);
    }
}
