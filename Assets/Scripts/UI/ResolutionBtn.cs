using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ResolutionBtn : MonoBehaviour
{
    [SerializeField]
    private Button leftBtn;
    [SerializeField]
    private Button rightBtn;

    private int screenSizeIndex = 0;

    [SerializeField]
    private TextMeshProUGUI text;

    private void SetResolution()
    {
        ScreenSize[] array = (ScreenSize[])System.Enum.GetValues(typeof(ScreenSize));
        screenSizeIndex = Mathf.Clamp(screenSizeIndex, 0, array.Length - 1);

        SettingManager.Instance.Set_ScreenSize(array[screenSizeIndex]);
        int[] screenSizes = SettingManager.Instance.GetScreenSize();
        text.text = screenSizes[0].ToString() +  " x " + screenSizes[1].ToString();
    }

    private void ResolutionUp()
    {
        screenSizeIndex--;
        SetResolution();
    }

    private void ResolutionDown()
    {
        screenSizeIndex++;
        SetResolution();
    }

    private void Awake()
    {
        if (leftBtn != null)
            leftBtn.onClick.AddListener(ResolutionDown);
        if (rightBtn != null)
            rightBtn.onClick.AddListener(ResolutionUp);

        screenSizeIndex = SettingManager.Instance.ScreenSizeIndex;
        SetResolution();
    }
}
