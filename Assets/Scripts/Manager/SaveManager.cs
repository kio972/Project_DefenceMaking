using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;

public class SaveManager : Singleton<SaveManager>
{
    // --- ���� ������ �����̸� ���� ("���ϴ� �̸�(����).json") --- //
    string settingDataFileName = "SettingData.json";
    string playerDataFileName = "PlayerData.json";

    // --- ����� Ŭ���� ���� --- //
    public SettingData settingData = new SettingData();

    public PlayerData playerData = null;

    private void SaveAssetData(PlayerData playerData)
    {
        playerData.curWave = GameManager.Instance.CurWave;
        playerData.curTime = GameManager.Instance.Timer;
        playerData.gold = GameManager.Instance.gold;
        playerData.herb1 = GameManager.Instance.herb1;
        playerData.herb2 = GameManager.Instance.herb2;
        playerData.herb3 = GameManager.Instance.herb3;
    }

    private void SaveTileData(PlayerData playerData)
    {
        playerData.tiles = new List<TileData>();
        foreach (TileNode node in NodeManager.Instance._ActiveNodes)
        {
            if (node.curTile != null)
                playerData.tiles.Add(node.curTile.GetTileData());
        }

        playerData.environments = new List<TileData>();
        foreach (Environment environment in NodeManager.Instance.environments)
        {
            TileData tile = new TileData();
            tile.id = environment.name.ToString().Replace("(Clone)", "");
            tile.row = environment._CurNode.row;
            tile.col = environment._CurNode.col;
            playerData.environments.Add(tile);
        }
    }

    public void SavePlayerData()
    {
        playerData = new PlayerData();

        SaveAssetData(playerData);
        SaveTileData(playerData);

        playerData.deckLists = new List<int>(GameManager.Instance.cardDeckController._CardDeck);
        playerData.cardIdes = new List<int>(GameManager.Instance.cardDeckController._HandCards);

        playerData.enemys = new List<BattlerData>();
        foreach (Battler enemy in GameManager.Instance.adventurersList)
            playerData.enemys.Add(enemy.GetData());
        playerData.allies = new List<BattlerData>();
        foreach (Battler monster in GameManager.Instance._MonsterList)
            playerData.allies.Add(monster.GetData());
        playerData.devil = GameManager.Instance.king.GetData();

        GameManager.Instance.research?.SaveData(playerData);
        GameManager.Instance.shop?.SaveData(playerData);
        QuestManager.Instance.SaveGame(playerData);

        SaveDataJsonCovnert(playerData, playerDataFileName);
    }

    public void SaveDataJsonCovnert(object data, string fileName)
    {
        string json = JsonConvert.SerializeObject(data);
        string filePath = Application.persistentDataPath + "/" + fileName;
        print(filePath);
        // �̹� ����� ������ �ִٸ� �����, ���ٸ� ���� ���� ����
        File.WriteAllText(filePath, json);
    }

    public void LoadPlayerData()
    {
        playerData = LoadDataJsonConvert<PlayerData>(playerDataFileName);
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

    // �ҷ�����
    public T LoadDataJsonConvert<T>(string fileName) where T : new()
    {
        string filePath = Application.persistentDataPath + "/" + fileName;

        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            return JsonConvert.DeserializeObject<T>(json);
        }
        else
            return GenerateNewData<T>();
    }

    public T LoadData<T>(string fileName) where T : new()
    {
        string filePath = Application.persistentDataPath + "/" + fileName;

        // ����� ������ �ִٸ�
        if (File.Exists(filePath))
        {
            // ����� ���� �о���� Json�� Ŭ���� �������� ��ȯ�ؼ� �Ҵ�
            string FromJsonData = File.ReadAllText(filePath);
            return JsonUtility.FromJson<T>(FromJsonData);
        }
        else
            return GenerateNewData<T>();
    }


    // �����ϱ�
    public void SaveData(object data, string fileName)
    {
        // Ŭ������ Json �������� ��ȯ (true : ������ ���� �ۼ�)
        string ToJsonData = JsonUtility.ToJson(data, true);
        string filePath = Application.persistentDataPath + "/" + fileName;
        print(filePath);
        // �̹� ����� ������ �ִٸ� �����, ���ٸ� ���� ���� ����
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
