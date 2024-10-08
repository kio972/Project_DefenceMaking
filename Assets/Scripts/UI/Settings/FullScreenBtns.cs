using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FullScreenBtns : SwitchButton
{
    protected override void OnValueChange()
    {
        base.OnValueChange();
        SettingManager.Instance.Set_FullScreen(value);
    }

    protected override void Init()
    {
        base.Init();
        SetBtn(SettingManager.Instance.screen_FullSize);
    }
}
