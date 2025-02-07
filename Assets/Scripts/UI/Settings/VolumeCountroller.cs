using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class VolumeCountroller : SliderWithText
{
    [SerializeField]
    private VolumeType targetType;

    public override void OnValueChange()
    {
        base.OnValueChange();

        if (slider == null)
            return;

        SettingManager.Instance.SetVolume(targetType, slider.value);
        SaveManager.Instance.SaveSettingData();
    }

    protected override void Awake()
    {
        if (slider != null)
            slider.value = SettingManager.Instance.GetVolume(targetType);
        base.Awake();
    }
}
