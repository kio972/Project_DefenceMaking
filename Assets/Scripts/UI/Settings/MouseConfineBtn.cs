using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MouseConfineBtn : SwitchButton
{
    protected override void OnValueChange()
    {
        base.OnValueChange();
        SettingManager.Instance.Set_MouseConfined(value);
    }

    protected override void Init()
    {
        base.Init();
        SetBtn(SettingManager.Instance.mouse_Confined);
    }
}
