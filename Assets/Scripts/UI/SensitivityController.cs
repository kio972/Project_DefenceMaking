using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SensitivityController : MonoBehaviour
{
    [SerializeField]
    private Slider slider;


    private void SilderUpdateCheck()
    {
        if (slider == null)
            return;

        SettingManager.Instance.mouseSensitivity = slider.value;
    }

    void Awake()
    {
        if (slider != null)
            slider.value = SettingManager.Instance.mouseSensitivity;
    }

    private void Update()
    {
        SilderUpdateCheck();
    }
}
