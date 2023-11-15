using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameSettingCallBtn : RotationEffect
{
    public override void Update()
    {
        base.Update();

        if (UIManager.Instance._OpendUICount != 0)
            return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SetSelected();
            SettingCanvas.Instance.CallSettings(true, true);
            SettingManager.Instance.SetLanguage();
        }
    }
}
