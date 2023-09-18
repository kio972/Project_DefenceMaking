using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ScreenSize
{
    Size_1920x1080,
    Size_1280x720,

}

public enum Languages
{
    korean,
    english,
}

public enum AutoPlaySetting
{
    off,
    setTile,
    always,
}

public class SettingManager : Singleton<SettingManager>
{
    //게임플레이
    public Languages language = Languages.korean;
    public AutoPlaySetting autoPlay = AutoPlaySetting.off;
    public float textSize = 1f;

    public float bgmVolume = 1f;
    public float fxVolume = 1f;
    public bool screen_FullSize = true;
    public ScreenSize screenSize = ScreenSize.Size_1920x1080;
    public float mouseSensitivity = 0.25f;
    public float MouseSensitivity
    {
        get
        {
            mouseSensitivity = Mathf.Clamp01(mouseSensitivity);
            return Mathf.Lerp(0.5f, 2.5f, mouseSensitivity);
        }
    }

    public int ScreenSizeIndex
    {
        get
        {
            return (int)screenSize;
        }
        set
        {
            if (System.Enum.IsDefined(typeof(ScreenSize), value))
            {
                screenSize = (ScreenSize)value;
            }
            else
            {
                Debug.LogError("Invalid ScreenSize index: " + value);
            }
        }
    }

    public void Set_FullScreen(bool value)
    {
        Screen.fullScreen = value;
        this.screen_FullSize = value;

        SaveManager.Instance.SaveSettingData();
    }

    public int[] GetScreenSize()
    {
        int width = 0;
        int height = 0;
        string screenSizeText = screenSize.ToString();
        screenSizeText = screenSizeText.Replace("Size_", "");
        string[] sizes = screenSizeText.Split("x");
        width = System.Convert.ToInt32(sizes[0]);
        height = System.Convert.ToInt32(sizes[1]);
        return new int[2] { width, height };
    }

    public void SetLanguage(Languages language)
    {
        LanguageText[] texts = FindObjectsOfType<LanguageText>(true);

        foreach (LanguageText text in texts)
        {
            text.ChangeLangauge(language);
        }
    }

    public void SetTextSize(float mult)
    {
        LanguageText[] texts = FindObjectsOfType<LanguageText>(true);

        foreach (LanguageText text in texts)
        {
            text.ChangeTextSize(mult);
        }
    }

    public void Set_ScreenSize(ScreenSize screenSize)
    {
        int width = 0;
        int height = 0;
        string screenSizeText = screenSize.ToString();
        screenSizeText = screenSizeText.Replace("Size_", "");
        string[] sizes = screenSizeText.Split("x");
        width = System.Convert.ToInt32(sizes[0]);
        height = System.Convert.ToInt32(sizes[1]);

        this.screenSize = screenSize;

        Screen.SetResolution(width, height, Screen.fullScreen);

        SaveManager.Instance.SaveSettingData();
    }

    public void Init()
    {
        SaveManager.Instance.LoadSettingData();
    }
}
