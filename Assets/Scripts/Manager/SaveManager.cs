using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveManager : Singleton<SaveManager>
{
    // --- 게임 데이터 파일이름 설정 ("원하는 이름(영문).json") --- //
    string settingDataFileName = "SettingData.json";

    // --- 저장용 클래스 변수 --- //
    public SettingData settingData = new SettingData();

    public void LoadSettingData()
    {
        settingData = LoadData<SettingData>(settingDataFileName);
        SettingManager.Instance.bgmVolume = settingData.volume_Bgm;
        SettingManager.Instance.fxVolume = settingData.volume_Fxs;
        SettingManager.Instance.screen_FullSize = settingData.fullScreen;
        SettingManager.Instance.ScreenSizeIndex = settingData.screenSizeIndex;
        SettingManager.Instance.mouseSensitivity = settingData.mouseSensitivity;
    }

    public void SaveSettingData()
    {
        settingData.volume_Bgm = SettingManager.Instance.bgmVolume;
        settingData.volume_Fxs = SettingManager.Instance.fxVolume;
        settingData.fullScreen = SettingManager.Instance.screen_FullSize;
        settingData.screenSizeIndex = SettingManager.Instance.ScreenSizeIndex;
        settingData.mouseSensitivity = SettingManager.Instance.mouseSensitivity;

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
