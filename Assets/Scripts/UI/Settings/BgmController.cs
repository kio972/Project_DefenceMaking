using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BgmController : SliderWithText
{
    public override void OnValueChange()
    {
        base.OnValueChange();

        if (slider == null)
            return;

        SettingManager.Instance.bgmVolume = slider.value;
        AudioManager.Instance.UpdateMusicVolume(SettingManager.Instance._BGMVolume);
        SaveManager.Instance.SaveSettingData();
    }

    protected override void Awake()
    {
        if (slider != null)
            slider.value = SettingManager.Instance.bgmVolume;
        base.Awake();
    }
}
