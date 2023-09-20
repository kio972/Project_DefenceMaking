using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIVolumeController : MonoBehaviour
{
    [SerializeField]
    private Slider slider;

    public void VolumeUpdateCheck()
    {
        if (slider == null)
            return;

        SettingManager.Instance.uiVolume = slider.value;
        SaveManager.Instance.SaveSettingData();
    }

    void Awake()
    {
        if (slider != null)
            slider.value = SettingManager.Instance.uiVolume;
    }
}
