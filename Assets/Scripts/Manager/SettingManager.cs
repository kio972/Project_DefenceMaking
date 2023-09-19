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
    public AutoPlaySetting autoPlay = AutoPlaySetting.off;
    public float textSize = 1f;
    #endregion

    #region ControlsSetting
    public float mouseSensitivity = 1f;
    public BindKey key_Camera_MoveUp = new BindKey(ControlKey.key_Camera_MoveUp, KeyCode.W);
    public BindKey key_Camera_MoveDown = new BindKey(ControlKey.key_Camera_MoveDown, KeyCode.S);
    public BindKey key_Camera_MoveLeft = new BindKey(ControlKey.key_Camera_MoveLeft, KeyCode.A);
    public BindKey key_Camera_MoveRight = new BindKey(ControlKey.key_Camera_MoveRight, KeyCode.D);

    public BindKey key_SpeedControl_Zero = new BindKey(ControlKey.key_SpeedControl_Zero, KeyCode.Space);
    public BindKey key_SpeedControl_One = new BindKey(ControlKey.key_SpeedControl_One, KeyCode.Alpha1);
    public BindKey key_SpeedControl_Double = new BindKey(ControlKey.key_SpeedControl_Double, KeyCode.Alpha2);

    public BindKey key_Camera_Reset = new BindKey(ControlKey.key_SpeedControl_Double, KeyCode.Return);
    public BindKey key_Camera_ResetAssist = new BindKey(ControlKey.key_Camera_ResetAssist, KeyCode.LeftShift);

    public BindKey key_BasicControl = new BindKey(ControlKey.key_BasicControl, KeyCode.Mouse0);
    public BindKey key_CancelControl = new BindKey(ControlKey.key_BasicControl, KeyCode.Mouse1);

    public BindKey key_RotateTile = new BindKey(ControlKey.key_RotateTile, KeyCode.R);

    public List<BindKey> bindKeys;

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

    private List<KeyCode> invalidKeys = new List<KeyCode>()
    {
        KeyCode.Escape, KeyCode.Mouse0, KeyCode.Mouse1
    };

    public bool IsValidKey(KeyCode value)
    {
        if (invalidKeys.Contains(value))
            return false;

        return true;
    }
    #endregion

    public float bgmVolume = 1f;
    public float fxVolume = 1f;
    public bool screen_FullSize = true;
    public ScreenSize screenSize = ScreenSize.Size_1920x1080;
    

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
    }

    public void Init()
    {
        SaveManager.Instance.LoadSettingData();
        InitControlKey();
    }
}
