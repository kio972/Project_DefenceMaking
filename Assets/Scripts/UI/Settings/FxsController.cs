using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FxsController : SliderWithText
{
    public override void OnValueChange()
    {
        base.OnValueChange();

        if (slider == null)
            return;

        SettingManager.Instance.fxVolume = slider.value;
        AudioManager.Instance.UpdateFxVolume(SettingManager.Instance._FxVolume);
        SaveManager.Instance.SaveSettingData();
    }

    protected override void Awake()
    {
        if (slider != null)
            slider.value = SettingManager.Instance.fxVolume;

        base.Awake();
    }
}
