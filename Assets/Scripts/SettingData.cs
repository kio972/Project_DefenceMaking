using System;


[Serializable]
public class SettingData
{
    public float volume_Master = 1f;
    public float volume_Bgm = 1f;
    public float volume_Fxs = 1f;
    public float volume_UI = 1f;
    public float volume_Amb = 1f;
    public bool muteOnBackground = false;

    public bool fullScreen = true;
    public bool mouseConfined = false;
    public bool fpsLimit = false;
    public int screenSizeIndex = 0;
    public float mouseSensitivity = 1f;
    public int language = 1;
    public int autoPlay = 0;

    public int key_Camera_MoveUp = 119;
    public int key_Camera_MoveDown = 115;
    public int key_Camera_MoveLeft = 97;
    public int key_Camera_MoveRight = 100;
    public int key_SpeedControl_Zero = 32;
    public int key_SpeedControl_One = 282;
    public int key_SpeedControl_Double = 283;
    public int key_deploy = 122;
    public int key_research = 120;
    public int key_shop = 99;

    public int stageState = 0;

    public bool showShortCut = false;
}
