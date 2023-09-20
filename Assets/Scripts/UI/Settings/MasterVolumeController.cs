using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MasterVolumeController : MonoBehaviour
{
    [SerializeField]
    private Slider slider;

    public void VolumeUpdateCheck()
    {
        if (slider == null)
            return;

        SettingManager.Instance.masterVolume = slider.value;
        AudioManager.Instance.UpdateFxVolume(SettingManager.Instance._FxVolume);
        AudioManager.Instance.UpdateMusicVolume(SettingManager.Instance._BGMVolume);
        SaveManager.Instance.SaveSettingData();
    }

    void Awake()
    {
        if (slider != null)
            slider.value = SettingManager.Instance.masterVolume;
    }
}
