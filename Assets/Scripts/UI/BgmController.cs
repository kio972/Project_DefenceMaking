using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BgmController : MonoBehaviour
{
    [SerializeField]
    private Slider slider;

    private void VolumeUpdateCheck()
    {
        if (slider == null)
            return;

        SettingManager.Instance.bgmVolume = slider.value;
        AudioManager.Instance.UpdateMusicVolume(SettingManager.Instance.bgmVolume);
    }

    void Awake()
    {
        if (slider != null)
            slider.value = SettingManager.Instance.bgmVolume;
    }

    private void Update()
    {
        VolumeUpdateCheck();
    }
}
