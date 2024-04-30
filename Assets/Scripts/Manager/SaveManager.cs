using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;

public class SaveManager : Singleton<SaveManager>
{
    // --- 게임 데이터 파일이름 설정 ("원하는 이름(영문).json") --- //
    string settingDataFileName = "SettingData.json";
    string playerDataFileName = "PlayerData.json";

    // --- 저장용 클래스 변수 --- //
    public SettingData settingData = new SettingData();

    public PlayerData playerData = new PlayerData();

    public void SavePlayerData()
    {
        string json = JsonConvert.SerializeObject(playerData);
        string filePath = Application.persistentDataPath + "/" + playerDataFileName;
        print(filePath);
        // 이미 저장된 파일이 있다면 덮어쓰고, 없다면 새로 만들어서 저장
        File.WriteAllText(filePath, json);
    }

    public void LoadPlayerData()
    {
        playerData = LoadData<PlayerData>(playerDataFileName);
    }

    public void LoadSettingData()
    {
        settingData = LoadData<SettingData>(settingDataFileName);
        SettingManager.Instance.masterVolume = settingData.volume_Master;
        SettingManager.Instance.bgmVolume = settingData.volume_Bgm;
        SettingManager.Instance.fxVolume = settingData.volume_Fxs;
        SettingManager.Instance.uiVolume = settingData.volume_UI;
        SettingManager.Instance.muteOnBackground = settingData.muteOnBackground;

        SettingManager.Instance.screen_FullSize = settingData.fullScreen;
        SettingManager.Instance.ScreenSizeIndex = settingData.screenSizeIndex;
        SettingManager.Instance.mouse_Confined = settingData.mouseConfined;
        SettingManager.Instance.fpsLimit = settingData.fpsLimit;

        SettingManager.Instance.mouseSensitivity = settingData.mouseSensitivity;
        SettingManager.Instance.language = (Languages)settingData.language;
        //SettingManager.Instance.autoPlay = (AutoPlaySetting)settingData.autoPlay;

        SettingManager.Instance.key_Camera_MoveUp.SetCurKey((KeyCode)settingData.key_Camera_MoveUp);
        SettingManager.Instance.key_Camera_MoveDown.SetCurKey((KeyCode)settingData.key_Camera_MoveDown);
        SettingManager.Instance.key_Camera_MoveLeft.SetCurKey((KeyCode)settingData.key_Camera_MoveLeft);
        SettingManager.Instance.key_Camera_MoveRight.SetCurKey((KeyCode)settingData.key_Camera_MoveRight);
        SettingManager.Instance.key_SpeedControl_Zero.SetCurKey((KeyCode)settingData.key_SpeedControl_Zero);
        SettingManager.Instance.key_SpeedControl_One.SetCurKey((KeyCode)settingData.key_SpeedControl_One);
        SettingManager.Instance.key_SpeedControl_Double.SetCurKey((KeyCode)settingData.key_SpeedControl_Double);
    }                               

    public void SaveSettingData()
    {
        settingData.volume_Master = SettingManager.Instance.masterVolume;
        settingData.volume_Bgm = SettingManager.Instance.bgmVolume;
        settingData.volume_Fxs = SettingManager.Instance.fxVolume;
        settingData.volume_UI = SettingManager.Instance.uiVolume;
        settingData.muteOnBackground = SettingManager.Instance.muteOnBackground;

        settingData.fullScreen = SettingManager.Instance.screen_FullSize;
        settingData.screenSizeIndex = SettingManager.Instance.ScreenSizeIndex;
        settingData.mouseConfined = SettingManager.Instance.mouse_Confined;
        settingData.fpsLimit = SettingManager.Instance.fpsLimit;

        settingData.mouseSensitivity = SettingManager.Instance.mouseSensitivity;
        settingData.language = (int)SettingManager.Instance.language;
        //settingData.autoPlay = (int)SettingManager.Instance.autoPlay;

        settingData.key_Camera_MoveUp = (int)SettingManager.Instance.key_Camera_MoveUp._CurKey;
        settingData.key_Camera_MoveDown = (int)SettingManager.Instance.key_Camera_MoveDown._CurKey;
        settingData.key_Camera_MoveLeft = (int)SettingManager.Instance.key_Camera_MoveLeft._CurKey;
        settingData.key_Camera_MoveRight = (int)SettingManager.Instance.key_Camera_MoveRight._CurKey;
        settingData.key_SpeedControl_Zero = (int)SettingManager.Instance.key_SpeedControl_Zero._CurKey;
        settingData.key_SpeedControl_One = (int)SettingManager.Instance.key_SpeedControl_One._CurKey;
        settingData.key_SpeedControl_Double = (int)SettingManager.Instance.key_SpeedControl_Double._CurKey;

        SaveData(settingData, settingDataFileName);
    }

    private T GenerateNewData<T>() where T : new()
    {
        return new T();
    }

    // 불러오기
    public T LoadData<T>(string fileName) where T : new()
    {
        string filePath = Application.persistentDataPath + "/" + fileName;

        // 저장된 게임이 있다면
        if (File.Exists(filePath))
        {
            // 저장된 파일 읽어오고 Json을 클래스 형식으로 전환해서 할당
            string FromJsonData = File.ReadAllText(filePath);
            return JsonUtility.FromJson<T>(FromJsonData);
        }
        else
            return GenerateNewData<T>();
    }


    // 저장하기
    public void SaveData(object data, string fileName)
    {
        // 클래스를 Json 형식으로 전환 (true : 가독성 좋게 작성)
        string ToJsonData = JsonUtility.ToJson(data, true);
        string filePath = Application.persistentDataPath + "/" + fileName;
        print(filePath);
        // 이미 저장된 파일이 있다면 덮어쓰고, 없다면 새로 만들어서 저장
        File.WriteAllText(filePath, ToJsonData);
    }

    public void Init()
    {
        settingData = LoadData<SettingData>(settingDataFileName);
    }

    public void OnApplicationQuit()
    {
        SaveSettingData();
    }
}
