using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SensitivityController : SliderWithText
{
    public override void OnValueChange()
    {
        base.OnValueChange();

        if (slider == null)
            return;

        SettingManager.Instance.mouseSensitivity = slider.value;
    }

    protected override void Awake()
    {
        if (slider != null)
            slider.value = SettingManager.Instance.mouseSensitivity;
        base.Awake();
    }
}
