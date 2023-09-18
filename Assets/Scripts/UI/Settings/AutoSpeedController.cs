using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoSpeedController : SpinnerButton
{
    protected override void Init()
    {
        base.Init();

        index = (int)SettingManager.Instance.autoPlay;
        OnValueChange();
    }

    protected override void OnValueChange()
    {
        base.OnValueChange();

        SettingManager.Instance.autoPlay = (AutoPlaySetting)index;
    }
}
