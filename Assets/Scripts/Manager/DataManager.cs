using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : Singleton<DataManager>
{
    #region CommonData
    private List<Dictionary<string, object>> _battler_Table;
    private List<Dictionary<string, object>> _timeRate_Table;
    private List<Dictionary<string, object>> _language_Table;
    private List<Dictionary<string, object>> _research_Table;
    private List<Dictionary<string, object>> _scriptsMalpongsun_Table;
    private List<Dictionary<string, object>> _buff_Table;
    private List<Dictionary<string, object>> _shop_Table;
    private List<Dictionary<string, object>> _deckList;

    public List<Dictionary<string, object>> battler_Table { get => _battler_Table; }
    public List<Dictionary<string, object>> timeRate_Table { get => _timeRate_Table; }
    public List<Dictionary<string, object>> language_Table { get => _language_Table; }
    public List<Dictionary<string, object>> research_Table { get => _research_Table; }
    public List<Dictionary<string, object>> scriptsMalpongsun_Table { get => _scriptsMalpongsun_Table; }
    public List<Dictionary<string, object>> shop_Table { get => _shop_Table; }
    public List<Dictionary<string, object>> deck_Table { get => _deckList; }

    public List<int> tileCard_Indexs { get; private set; }
    public List<int> monsterCard_Indexs { get; private set; }
    public List<int> trapCard_Indexs { get; private set; }
    public List<int> environmentCard_Indexs { get; private set; }
    public List<int> herbCard_Indexs { get; private set; }
    public List<int> pathCard_Indexs { get; private set; }
    public List<int> roomCard_Indexs { get; private set; }
    public List<int> roomPartCard_Index { get; private set; }
    public List<int> roomTypeCard_Indexs { get; private set; }
    public Dictionary<string, int> deckListIndex { get; private set; }
    public Dictionary<string, Dictionary<string, object>> battlerDic { get; private set; }
    public Dictionary<string, Dictionary<string, object>> shopListDic { get; private set; }
    public Dictionary<string, Dictionary<string, object>> languageDic { get; private set; }
    #endregion

    #region StageData
    private List<Dictionary<string, object>> _wave_Table;
    private List<Dictionary<string, object>> _scripts_Table;
    private List<Dictionary<string, object>> _quest_Table;
    private List<Dictionary<string, object>> _questMessage_Table;
    private List<Dictionary<string, object>> _start_deckTable;
    private List<Dictionary<string, object>> _hiddenTile_Table;

    public List<Dictionary<string, object>> wave_Table { get => _wave_Table; }
    public List<Dictionary<string, object>> quest_Table { get => _quest_Table; }
    public List<Dictionary<string, object>> questMessage_Table { get => _questMessage_Table; }
    public List<Dictionary<string, object>> start_deckTable { get => _start_deckTable; }
    public List<Dictionary<string, object>> scripts_Table { get => _scripts_Table; }
    public List<Dictionary<string, object>> hiddenTile_Table { get => _hiddenTile_Table; }

    public Dictionary<string, List<Dictionary<string, object>>> scriptsDic = null;
    public Dictionary<int, List<WaveData>> waveLevelTable { get; private set; }
    #endregion

    private void InitLevelTable()
    {
        waveLevelTable = new Dictionary<int, List<WaveData>>();
        foreach (Dictionary<string, object> data in _wave_Table)
        {
            int level = Convert.ToInt32(data["level"]);
            string adventurerName = data["enemy"].ToString();
            int number = Convert.ToInt32(data["number"]);
            WaveData waveData = new WaveData(adventurerName, number);
            if (!waveLevelTable.ContainsKey(level))
                waveLevelTable.Add(level, new List<WaveData>());
            waveLevelTable[level].Add(waveData);
        }
    }

    private void InitScripts()
    {
        scriptsDic = new Dictionary<string, List<Dictionary<string, object>>>();
        foreach (Dictionary<string, object> script in _scripts_Table)
        {
            string id = script["id"].ToString();
            if (!scriptsDic.ContainsKey(id))
                scriptsDic.Add(id, new List<Dictionary<string, object>>());

            scriptsDic[id].Add(script);
        }
    }

    public List<Dictionary<string, object>> GetScripts(string key)
    {
        if (scriptsDic == null)
            InitScripts();

        if (!scriptsDic.ContainsKey(key))
            return null;

        return scriptsDic[key];
    }


    public List<int> RoomTypeCard_Indexs
    {
        get
        {
            if(roomTypeCard_Indexs == null)
            {
                roomTypeCard_Indexs = new List<int>(roomCard_Indexs);
                foreach (int val in roomPartCard_Index)
                    roomTypeCard_Indexs.Add(val);
            }

            return roomTypeCard_Indexs;
        }
    }

    public string GetDescription(string key)
    {
        //int index = UtilHelper.Find_Data_Index(key, language_Table, "id");
        //if (index == -1)
        //    return key;
        //string language = SettingManager.Instance.language.ToString();
        //return language_Table[index][language].ToString();

        if (!languageDic.ContainsKey(key))
            return key;

        return languageDic[key][SettingManager.Instance.language.ToString()].ToString();
    }

    private Dictionary<int, Dictionary<char, Dictionary<string, object>>> sortedBuffTable = null;

    public Dictionary<int, Dictionary<char, Dictionary<string, object>>> BuffTable
    {
        get
        {
            if(sortedBuffTable == null)
            {
                sortedBuffTable = new Dictionary<int, Dictionary<char, Dictionary<string, object>>>();
                foreach (Dictionary<string, object> value in _buff_Table)
                {
                    string[] split = value["id"].ToString().Split('_');
                    int loop = Convert.ToInt32(split[0]);
                    char choice = split[1][0];
                    if (!sortedBuffTable.ContainsKey(loop))
                        sortedBuffTable.Add(loop, new Dictionary<char, Dictionary<string, object>>());
                    sortedBuffTable[loop].Add(choice, value);
                }
            }

            return sortedBuffTable;
        }
    }

    public Dictionary<string, object> GetMalpoongsunScript(string id)
    {
        int index = UtilHelper.Find_Data_Index(id, _scriptsMalpongsun_Table, "id");
        if (index == -1)
            return null;

        return _scriptsMalpongsun_Table[index];
    }

    private void SortShopList()
    {
        shopListDic = new Dictionary<string, Dictionary<string, object>>();
        foreach (var item in _shop_Table)
            shopListDic.Add(item["id"].ToString(), item);
    }

    private void SortDeckList()
    {
        tileCard_Indexs = new List<int>();
        monsterCard_Indexs = new List<int>();
        trapCard_Indexs = new List<int>();

        pathCard_Indexs = new List<int>();
        roomCard_Indexs = new List<int>();
        roomPartCard_Index = new List<int>();
        environmentCard_Indexs = new List<int>();
        herbCard_Indexs = new List<int>();

        deckListIndex = new Dictionary<string, int>();

        for (int i = 0; i < _deckList.Count; i++)
        {
            if (string.IsNullOrEmpty(_deckList[i]["prefab"].ToString()))
                continue;

            if (_deckList[i]["cardtype"].ToString() == "tile")
                tileCard_Indexs.Add(i);
            if (_deckList[i]["cardtype"].ToString() == "monster")
                monsterCard_Indexs.Add(i);
            if (_deckList[i]["cardtype"].ToString() == "trap")
                trapCard_Indexs.Add(i);

            if (_deckList[i]["type"].ToString() == "road")
                pathCard_Indexs.Add(i);
            if (_deckList[i]["type"].ToString() == "room")
                roomCard_Indexs.Add(i);
            if (_deckList[i]["type"].ToString() == "roomPart")
                roomPartCard_Index.Add(i);
            if (_deckList[i]["type"].ToString() == "environment")
                environmentCard_Indexs.Add(i);
            if (_deckList[i]["type"].ToString() == "herb")
                herbCard_Indexs.Add(i);

            deckListIndex.Add(_deckList[i]["id"].ToString(), i);
        }
    }

    private void SortLanguageList()
    {
        languageDic = new Dictionary<string, Dictionary<string, object>>();
        foreach (var item in _language_Table)
            languageDic.Add(item["id"].ToString(), item);
    }

    private void SortBattlerList()
    {
        battlerDic = new Dictionary<string, Dictionary<string, object>>();
        foreach (var item in _battler_Table)
            battlerDic.Add(item["id"].ToString(), item);
    }

    // csv파일 주소(Resource폴더 내)
    //private readonly string wave_Table_DataPath = "Data/waveData";
    private readonly string deckList_DataPath = "Data/deckList";
    private readonly string battler_Table_DataPath = "Data/battlerTable";
    //private readonly string timeRate_Table_DataPath = "Data/timeData";
    private readonly string language_Table_DataPath = "Data/languageData";
    //private readonly string scripts_Table_DataPath = "Data/scriptData";
    private readonly string buff_Table_DataPath = "Data/buffData";
    private readonly string research_Table_DataPath = "Data/researchData";
    private readonly string scriptsMalpongsun_Table_DataPath = "Data/scriptMalpongsunData";
    //private readonly string quest_Table_DataPath = "Data/questData";
    //private readonly string questMessage_Table_DataPath = "Data/questMessageData";
    private readonly string shop_Table_DataPath = "Data/shopListData";

    public void SetStageData(TextAsset waveData, TextAsset timeRateData, TextAsset deckListData, TextAsset scriptData, TextAsset questData, TextAsset questMessageData, TextAsset hiddenTileData)
    {
        _wave_Table = CSVLoader.LoadCSV(waveData);
        _timeRate_Table = CSVLoader.LoadCSV(timeRateData);
        _start_deckTable = CSVLoader.LoadCSV(deckListData);
        _scripts_Table = CSVLoader.LoadCSV(scriptData);
        _quest_Table = CSVLoader.LoadCSV(questData);
        _questMessage_Table = CSVLoader.LoadCSV(questMessageData);

        InitLevelTable();
        InitScripts();
    }

    private void Init()
    {
        // csv파일 불러오는 함수
        //wave_Table = CSVLoader.LoadCSV(Resources.Load<TextAsset>(wave_Table_DataPath));
        _deckList = CSVLoader.LoadCSV(Resources.Load<TextAsset>(deckList_DataPath));
        _battler_Table = CSVLoader.LoadCSV(Resources.Load<TextAsset>(battler_Table_DataPath));
        //timeRate_Table = CSVLoader.LoadCSV(Resources.Load<TextAsset>(timeRate_Table_DataPath));
        _language_Table = CSVLoader.LoadCSV(Resources.Load<TextAsset>(language_Table_DataPath));
        //scripts_Table = CSVLoader.LoadCSV(Resources.Load<TextAsset>(scripts_Table_DataPath));
        _buff_Table = CSVLoader.LoadCSV(Resources.Load<TextAsset>(buff_Table_DataPath));
        _research_Table = CSVLoader.LoadCSV(Resources.Load<TextAsset>(research_Table_DataPath));
        _scriptsMalpongsun_Table = CSVLoader.LoadCSV(Resources.Load<TextAsset>(scriptsMalpongsun_Table_DataPath));
        //quest_Table = CSVLoader.LoadCSV(Resources.Load<TextAsset>(quest_Table_DataPath));
        //questMessage_Table = CSVLoader.LoadCSV(Resources.Load<TextAsset>(questMessage_Table_DataPath));
        _shop_Table = CSVLoader.LoadCSV(Resources.Load<TextAsset>(shop_Table_DataPath));

        SortDeckList();
        SortShopList();
        SortLanguageList();
        SortBattlerList();
    }
}