using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ScreenSize
{
    Size_1920x1080,
    Size_1600x900,
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

public enum ControlKey
{
    None,
    key_Camera_MoveUp,
    key_Camera_MoveDown,
    key_Camera_MoveLeft,
    key_Camera_MoveRight,
    key_SpeedControl_Zero,
    key_SpeedControl_One,
    key_SpeedControl_Double,

    key_RotateTile,
    key_Camera_Reset,
    key_Camera_ResetAssist,

    key_BasicControl,
    key_CancelControl,

    key_Deploy,
    key_Research,
    key_Shop,

    key_Draw,
}

public class BindKey
{
    private ControlKey controlKey;
    public ControlKey _ControlKey { get => controlKey; }
    private KeyCode curKey;
    public KeyCode _CurKey { get => curKey; }

    public void SetCurKey(KeyCode value)
    {
        curKey = value;
    }

    public BindKey(ControlKey controlKey, KeyCode curKey)
    {
        this.controlKey = controlKey;
        this.curKey = curKey;
    }
}

public class SettingManager : Singleton<SettingManager>
{
    #region GamePlaySetting
    public Languages language = Languages.korean;
    public AutoPlaySetting autoPlay = AutoPlaySetting.setTile;
    public float textSize = 1f;
    public bool showShortCut = false;
    #endregion

    #region ControlsSetting
    public float mouseSensitivity = 1f;
    public BindKey key_Camera_MoveUp = new BindKey(ControlKey.key_Camera_MoveUp, KeyCode.W);
    public BindKey key_Camera_MoveDown = new BindKey(ControlKey.key_Camera_MoveDown, KeyCode.S);
    public BindKey key_Camera_MoveLeft = new BindKey(ControlKey.key_Camera_MoveLeft, KeyCode.A);
    public BindKey key_Camera_MoveRight = new BindKey(ControlKey.key_Camera_MoveRight, KeyCode.D);

    public BindKey key_SpeedControl_Zero = new BindKey(ControlKey.key_SpeedControl_Zero, KeyCode.Space);
    public BindKey key_SpeedControl_One = new BindKey(ControlKey.key_SpeedControl_One, KeyCode.F1);
    public BindKey key_SpeedControl_Double = new BindKey(ControlKey.key_SpeedControl_Double, KeyCode.F2);

    public BindKey key_Camera_Reset = new BindKey(ControlKey.key_SpeedControl_Double, KeyCode.Return);
    public BindKey key_Camera_ResetAssist = new BindKey(ControlKey.key_Camera_ResetAssist, KeyCode.LeftShift);

    public BindKey key_BasicControl = new BindKey(ControlKey.key_BasicControl, KeyCode.Mouse0);
    public BindKey key_CancelControl = new BindKey(ControlKey.key_BasicControl, KeyCode.Mouse1);

    public BindKey key_RotateLeft = new BindKey(ControlKey.key_RotateTile, KeyCode.Q);
    public BindKey key_RotateRight = new BindKey(ControlKey.key_RotateTile, KeyCode.E);

    public BindKey key_Deploy = new BindKey(ControlKey.key_Deploy, KeyCode.Z);
    public BindKey key_Research = new BindKey(ControlKey.key_Research, KeyCode.X);
    public BindKey key_Shop = new BindKey(ControlKey.key_Shop, KeyCode.C);

    public BindKey key_Draw = new BindKey(ControlKey.key_Draw, KeyCode.F);

    public int stageState = 0;

    public List<BindKey> bindKeys;

    private HashSet<KeyCode> invalidKeys = new HashSet<KeyCode>()
    {
        KeyCode.Mouse0, KeyCode.Mouse1, KeyCode.Mouse2, KeyCode.Mouse3, KeyCode.Mouse4, KeyCode.Mouse5, KeyCode.Mouse6,
        KeyCode.Escape, KeyCode.Return, KeyCode.Backspace, KeyCode.LeftWindows, KeyCode.RightWindows,
        KeyCode.Pause, KeyCode.Print, KeyCode.ScrollLock, KeyCode.Break, KeyCode.Numlock,
        KeyCode.Tab, KeyCode.CapsLock, KeyCode.LeftShift, KeyCode.RightShift, KeyCode.LeftControl, KeyCode.RightControl,
        KeyCode.LeftAlt, KeyCode.RightAlt, KeyCode.Menu,
    };

    public BindKey GetBindKey(ControlKey taregt)
    {
        foreach (BindKey bindkey in bindKeys)
        {
            if (bindkey._ControlKey == taregt)
                return bindkey;
        }
        return null;
    }

    public KeyCode GetKey(ControlKey taregt)
    {
        foreach (BindKey bindkey in bindKeys)
        {
            if (bindkey._ControlKey == taregt)
                return bindkey._CurKey;
        }
        return KeyCode.None;
    }

    public void SetKey(ControlKey taregt, KeyCode value)
    {
        foreach (BindKey bindkey in bindKeys)
        {
            if (bindkey._ControlKey == taregt)
            {
                bindkey.SetCurKey(value);
                SaveManager.Instance.SaveSettingData();
                return;
            }
        }
    }

    public bool IsValidKey(KeyCode value)
    {
        if (invalidKeys.Contains(value))
            return false;

        return true;
    }
    #endregion

    #region DisplaySetting
    public ScreenSize screenSize = ScreenSize.Size_1920x1080;
    public bool screen_FullSize = true;
    public bool mouse_Confined = false;
    public bool fpsLimit = false;

    public int ScreenSizeIndex
    {
        get { return (int)screenSize; }
        set
        {
            if (System.Enum.IsDefined(typeof(ScreenSize), value))
                screenSize = (ScreenSize)value;
            else
                Debug.LogError("Invalid ScreenSize index: " + value);
        }
    }

    #endregion

    #region AudioSetting
    public float masterVolume = 1f;
    public float bgmVolume = 1f;
    public float fxVolume = 1f;
    public float uiVolume = 1f;
    public bool muteOnBackground = false;

    public float _BGMVolume { get { return bgmVolume * masterVolume; } }
    public float _FxVolume { get { return fxVolume * masterVolume; } }
    public float _UIVolume { get { return uiVolume * masterVolume; } }

    #endregion

    public string nextScene = "";

    public void Set_FPSLimit(bool value)
    {
        if (value)
            Application.targetFrameRate = 60;
        else
            Application.targetFrameRate = -1;

        this.fpsLimit = value;
        SaveManager.Instance.SaveSettingData();
    }

    public void Set_MouseConfined(bool value)
    {
        if(value)
            Cursor.lockState = CursorLockMode.Confined;
        else
            Cursor.lockState = CursorLockMode.None;

        this.mouse_Confined = value;
        SaveManager.Instance.SaveSettingData();
    }

    public void Set_FullScreen(bool value)
    {
        Screen.fullScreen = value;
        this.screen_FullSize = value;

        SaveManager.Instance.SaveSettingData();
    }

    public void Set_ShowShortcut(bool value)
    {
        this.showShortCut = value;
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

    public void SetLanguage()
    {
        SetLanguage(language);
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

    private void InitControlKey()
    {
        bindKeys = new List<BindKey>();
        bindKeys.Add(key_Camera_MoveUp);
        bindKeys.Add(key_Camera_MoveDown);
        bindKeys.Add(key_Camera_MoveLeft);
        bindKeys.Add(key_Camera_MoveRight);
        bindKeys.Add(key_SpeedControl_Zero);
        bindKeys.Add(key_SpeedControl_One);
        bindKeys.Add(key_SpeedControl_Double);
        bindKeys.Add(key_Deploy);
        bindKeys.Add(key_Research);
        bindKeys.Add(key_Shop);
    }

    public void Init()
    {
        SaveManager.Instance.LoadSettingData();
        InitControlKey();
        Set_ScreenSize(screenSize);
        Set_FullScreen(screen_FullSize);
        SetLanguage(language);
        SetTextSize(textSize);
    }
}
