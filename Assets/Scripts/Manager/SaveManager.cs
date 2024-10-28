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

    private void SaveSpanwerData(PlayerData playerData)
    {
        playerData.spawners = new List<SpawnerData>();
        foreach(MonsterSpawner spawner in GameManager.Instance.monsterSpawner)
        {
            SpawnerData spawnerData = new SpawnerData();
            spawnerData.row = spawner.CurTile.row;
            spawnerData.col = spawner.CurTile.col;
            spawnerData.spawnerId = spawner._TargetName;
            spawnerData.spawnerCool = spawner._CurCoolTime;
            playerData.spawners.Add(spawnerData);
        }
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

        playerData.hiddenTiles = new List<TileData>();
        foreach(TileNode tileHidden in NodeManager.Instance.hiddenTiles)
        {
            TileData tile = new TileData();
            tile.id = "HiddenTile";
            tile.row = tileHidden.row;
            tile.col = tileHidden.col;
            playerData.hiddenTiles.Add(tile);
        }

        playerData.nextHiddenTileCount = GameManager.Instance.mapBuilder.curTileSetCount;
    }

    public void SavePlayerData()
    {
        playerData = new PlayerData();

        SaveAssetData(playerData);
        SaveTileData(playerData);
        SaveSpanwerData(playerData);

        playerData.deckLists = new List<int>(GameManager.Instance.cardDeckController.cardDeck);
        playerData.cardIdes = new List<int>(GameManager.Instance.cardDeckController.handCards);

        playerData.enemys = new List<BattlerData>();
        foreach (Battler enemy in GameManager.Instance.adventurersList)
        {
            if(enemy is ISaveLoadBattler battler)
                playerData.enemys.Add(battler.GetData());
        }

        playerData.allies = new List<BattlerData>();
        foreach (Battler monster in GameManager.Instance._MonsterList)
        {
            if (monster is ISaveLoadBattler battler)
                playerData.allies.Add(battler.GetData());
        }

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
        // 이미 저장된 파일이 있다면 덮어쓰고, 없다면 새로 만들어서 저장
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

        SettingManager.Instance.stageState = settingData.stageState;
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

        settingData.stageState = SettingManager.Instance.stageState;

        SaveData(settingData, settingDataFileName);
    }

    private T GenerateNewData<T>() where T : new()
    {
        return new T();
    }

    // 불러오기
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
