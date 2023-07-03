using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResolutionBtn : MonoBehaviour
{
    [SerializeField]
    private Button leftBtn;
    [SerializeField]
    private Button rightBtn;

    private int screenSizeIndex = 0;

    private void SetResolution()
    {
        ScreenSize[] array = (ScreenSize[])System.Enum.GetValues(typeof(ScreenSize));
        screenSizeIndex = Mathf.Clamp(screenSizeIndex, 0, array.Length - 1);

        SettingManager.Instance.Set_ScreenSize(array[screenSizeIndex]);
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
    }
}
