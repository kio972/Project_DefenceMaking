using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIVolumeController : SliderWithText
{
    public override void OnValueChange()
    {
        base.OnValueChange();

        if (slider == null)
            return;

        SettingManager.Instance.uiVolume = slider.value;
        SaveManager.Instance.SaveSettingData();
    }

    protected override void Awake()
    {
        if (slider != null)
            slider.value = SettingManager.Instance.uiVolume;
        base.Awake();
    }
}
