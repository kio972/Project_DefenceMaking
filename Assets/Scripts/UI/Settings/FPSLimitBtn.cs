using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FPSLimitBtn : SwitchButton
{
    protected override void OnValueChange()
    {
        base.OnValueChange();
        SettingManager.Instance.Set_FPSLimit(value);
    }

    protected override void Init()
    {
        base.Init();
        SetBtn(SettingManager.Instance.fpsLimit);
    }
}
