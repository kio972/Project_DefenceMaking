using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LanguageController : SpinnerButton
{
    protected override void Init()
    {
        base.Init();

        index = (int)SettingManager.Instance.language;
        OnValueChange();
    }

    private void SetLanguage()
    {
        Languages language = (Languages)index;
        SettingManager.Instance.SetLanguage(language);
    }

    protected override void OnValueChange()
    {
        base.OnValueChange();

        SettingManager.Instance.language = (Languages)index;
        SetLanguage();
    }
}
