using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MasterVolumeController : SliderWithText
{
    public override void OnValueChange()
    {
        base.OnValueChange();
        if (slider == null)
            return;

        SettingManager.Instance.masterVolume = slider.value;
        AudioManager.Instance.UpdateFxVolume(SettingManager.Instance._FxVolume);
        AudioManager.Instance.UpdateMusicVolume(SettingManager.Instance._BGMVolume);
        SaveManager.Instance.SaveSettingData();
    }
    protected override void Awake()
    {
        if (slider != null)
            slider.value = SettingManager.Instance.masterVolume;
        base.Awake();
    }
}
