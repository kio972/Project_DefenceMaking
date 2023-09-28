using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SliderWithText : MonoBehaviour
{
    [SerializeField]
    protected Slider slider;
    [SerializeField]
    protected TextMeshProUGUI sliderText;

    public virtual void OnValueChange()
    {
        if (slider == null)
            return;
        float curValue = (slider.value - slider.minValue) / (slider.maxValue - slider.minValue);
        //float curValue = Mathf.Lerp(slider.minValue, slider.maxValue, slider.value);
        curValue = Mathf.Round(curValue * 100f);
        if(sliderText != null)
            sliderText.text = curValue.ToString();
    }

    protected virtual void Awake()
    {
        OnValueChange();
    }
}
