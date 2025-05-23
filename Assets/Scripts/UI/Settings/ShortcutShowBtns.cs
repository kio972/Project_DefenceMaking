using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShortcutShowBtns : SwitchButton
{
    protected override void OnValueChange()
    {
        base.OnValueChange();
        SettingManager.Instance.Set_ShowShortcut(value);
    }

    protected override void Init()
    {
        base.Init();
        SetBtn(SettingManager.Instance.showShortCut);
    }
}
