using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextSizeController : MonoBehaviour
{
    [SerializeField]
    private Slider slider;


    public void SilderUpdateCheck()
    {
        if (slider == null)
            return;

        SettingManager.Instance.textSize = slider.value;
        SettingManager.Instance.SetTextSize(SettingManager.Instance.textSize);
    }

    void Awake()
    {
        if (slider != null)
            slider.value = SettingManager.Instance.textSize;
    }
}
