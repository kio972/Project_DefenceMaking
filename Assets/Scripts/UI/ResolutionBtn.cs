using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ResolutionBtn : SpinnerButton
{
    protected override void OnValueChange()
    {
        base.OnValueChange();

        SettingManager.Instance.ScreenSizeIndex = index;
        SettingManager.Instance.Set_ScreenSize((ScreenSize)index);
        int[] screenSizes = SettingManager.Instance.GetScreenSize();
        text.text = screenSizes[0].ToString() + " x " + screenSizes[1].ToString();
    }

    protected override void Init()
    {
        base.Init();
        index = SettingManager.Instance.ScreenSizeIndex;
        OnValueChange();
    }
}
