using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ScreenSize
{
    Size_1920x1080,
    Size_1280x720,

}

public class SettingManager : Singleton<SettingManager>
{
    public float bgmVolume = 1f;
    public float fxVolume = 1f;
    public bool screen_FullSize = true;
    public ScreenSize screenSize = ScreenSize.Size_1920x1080;

    

    public void Set_FullScreen(bool value)
    {
        Screen.fullScreen = value;
        this.screen_FullSize = value;
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
    }

    private void Load_SettingData()
    {
        //설정 데이터에서 데이터 불러오기
    }

    public void Init()
    {
        Load_SettingData();
    }
}
