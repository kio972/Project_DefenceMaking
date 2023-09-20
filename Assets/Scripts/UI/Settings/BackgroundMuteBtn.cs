using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackgroundMuteBtn : SwitchButton
{
    protected override void OnValueChange()
    {
        base.OnValueChange();
        SettingManager.Instance.muteOnBackground = value;
        SaveManager.Instance.SaveSettingData();
    }

    protected override void Init()
    {
        base.Init();
        SetBtn(SettingManager.Instance.muteOnBackground);
    }
}
