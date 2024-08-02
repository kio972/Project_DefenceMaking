using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : Singleton<DataManager>
{
    private List<Dictionary<string, object>> wave_Table;
    private List<Dictionary<string, object>> deckList;
    private List<Dictionary<string, object>> battler_Table;
    private List<Dictionary<string, object>> timeRate_Table;
    private List<Dictionary<string, object>> language_Table;
    private List<Dictionary<string, object>> scripts_Table;

    private List<Dictionary<string, object>> buff_Table;

    private List<Dictionary<string, object>> research_Table;
    private List<Dictionary<string, object>> scriptsMalpongsun_Table;

    private List<Dictionary<string, object>> quest_Table;
    private List<Dictionary<string, object>> questMessage_Table;

    private List<Dictionary<string, object>> shop_Table;

    public List<Dictionary<string, object>> Wave_Table { get => wave_Table; }
    public List<Dictionary<string, object>> Deck_Table { get => deckList; }
    public List<Dictionary<string, object>> Battler_Table { get => battler_Table; }
    public List<Dictionary<string, object>> TimeRate_Table { get => timeRate_Table; }
    public List<Dictionary<string, object>> Language_Table { get => language_Table; }
    public List<Dictionary<string, object>> Research_Table { get => research_Table; }
    public List<Dictionary<string, object>> ScriptsMalpongsun_Table { get => scriptsMalpongsun_Table; }
    public List<Dictionary<string, object>> Quest_Table { get => quest_Table; }
    public List<Dictionary<string, object>> QuestMessage_Table { get => questMessage_Table; }
    public List<Dictionary<string, object>> Shop_Table { get => shop_Table; }

    public List<int> tileCard_Indexs { get; private set; }
    public List<int> monsterCard_Indexs { get; private set; }
    public List<int> trapCard_Indexs { get; private set; }
    public List<int> environmentCard_Indexs { get; private set; }
    public List<int> herbCard_Indexs { get; private set; }

    public List<int> pathCard_Indexs { get; private set; }
    public List<int> roomCard_Indexs { get; private set; }
    public List<int> roomPartCard_Index { get; private set; }
    public List<int> roomTypeCard_Indexs { get; private set; }

    public List<Dictionary<string, object>> Scripts_Table { get => scripts_Table; }

    public Dictionary<string, List<Dictionary<string, object>>> scriptsDic = null;

    private Dictionary<int, List<WaveData>> waveLevelTable = null;

    public Dictionary<int, List<WaveData>> WaveLevelTable
    {
        get
        {
            if(waveLevelTable == null)
            {
                waveLevelTable = new Dictionary<int, List<WaveData>>();
                foreach(Dictionary<string, object> data in wave_Table)
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

            return waveLevelTable;
        }
    }

    private void InitScripts()
    {
        scriptsDic = new Dictionary<string, List<Dictionary<string, object>>>();
        foreach (Dictionary<string, object> script in scripts_Table)
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
        int index = UtilHelper.Find_Data_Index(key, language_Table, "id");
        if (index == -1)
            return key;

        string language = SettingManager.Instance.language.ToString();
        return language_Table[index][language].ToString();
    }

    private List<int> Find_Typeof_Index(List<Dictionary<string, object>> table, string key, string value)
    {
        List<int> indexs = new List<int>();
        for (int i = 0; i < table.Count; i++)
        {
            if (deckList[i][key].ToString() == value)
                indexs.Add(i);
        }
        return indexs;
    }

    private Dictionary<int, Dictionary<char, Dictionary<string, object>>> sortedBuffTable = null;

    public Dictionary<int, Dictionary<char, Dictionary<string, object>>> BuffTable
    {
        get
        {
            if(sortedBuffTable == null)
            {
                sortedBuffTable = new Dictionary<int, Dictionary<char, Dictionary<string, object>>>();
                foreach (Dictionary<string, object> value in buff_Table)
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
        int index = UtilHelper.Find_Data_Index(id, scriptsMalpongsun_Table, "id");
        if (index == -1)
            return null;

        return scriptsMalpongsun_Table[index];
    }

    public Dictionary<string, Dictionary<string, object>> shopListDic { get; private set; }

    // csv파일 주소(Resource폴더 내)
    private readonly string wave_Table_DataPath = "Data/waveData";
    private readonly string deckList_DataPath = "Data/deckList";
    private readonly string battler_Table_DataPath = "Data/battlerTable";
    private readonly string timeRate_Table_DataPath = "Data/timeData";
    private readonly string language_Table_DataPath = "Data/languageData";
    private readonly string scripts_Table_DataPath = "Data/scriptData";
    private readonly string buff_Table_DataPath = "Data/buffData";
    private readonly string research_Table_DataPath = "Data/researchData";
    private readonly string scriptsMalpongsun_Table_DataPath = "Data/scriptMalpongsunData";
    private readonly string quest_Table_DataPath = "Data/questData";
    private readonly string questMessage_Table_DataPath = "Data/questMessageData";
    private readonly string shop_Table_DataPath = "Data/shopListData";

    private void SortShopList()
    {
        shopListDic = new Dictionary<string, Dictionary<string, object>>();
        foreach(var item in shop_Table)
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

        for(int i = 0; i < deckList.Count; i++)
        {
            if(deckList[i]["cardtype"].ToString() == "tile")
                tileCard_Indexs.Add(i);
            if (deckList[i]["cardtype"].ToString() == "monster")
                monsterCard_Indexs.Add(i);
            if (deckList[i]["cardtype"].ToString() == "trap")
                trapCard_Indexs.Add(i);

            if (deckList[i]["type"].ToString() == "road")
                pathCard_Indexs.Add(i);
            if (deckList[i]["type"].ToString() == "room")
                roomCard_Indexs.Add(i);
            if (deckList[i]["type"].ToString() == "roomPart")
                roomPartCard_Index.Add(i);
            if (deckList[i]["type"].ToString() == "environment")
                environmentCard_Indexs.Add(i);
            if (deckList[i]["type"].ToString() == "herb")
                herbCard_Indexs.Add(i);
        }
    }

    private void Init()
    {
        // csv파일 불러오는 함수
        wave_Table = CSVLoader.LoadCSV(Resources.Load<TextAsset>(wave_Table_DataPath));
        deckList = CSVLoader.LoadCSV(Resources.Load<TextAsset>(deckList_DataPath));
        battler_Table = CSVLoader.LoadCSV(Resources.Load<TextAsset>(battler_Table_DataPath));
        timeRate_Table = CSVLoader.LoadCSV(Resources.Load<TextAsset>(timeRate_Table_DataPath));
        language_Table = CSVLoader.LoadCSV(Resources.Load<TextAsset>(language_Table_DataPath));
        scripts_Table = CSVLoader.LoadCSV(Resources.Load<TextAsset>(scripts_Table_DataPath));
        buff_Table = CSVLoader.LoadCSV(Resources.Load<TextAsset>(buff_Table_DataPath));
        research_Table = CSVLoader.LoadCSV(Resources.Load<TextAsset>(research_Table_DataPath));
        scriptsMalpongsun_Table = CSVLoader.LoadCSV(Resources.Load<TextAsset>(scriptsMalpongsun_Table_DataPath));
        quest_Table = CSVLoader.LoadCSV(Resources.Load<TextAsset>(quest_Table_DataPath));
        questMessage_Table = CSVLoader.LoadCSV(Resources.Load<TextAsset>(questMessage_Table_DataPath));
        shop_Table = CSVLoader.LoadCSV(Resources.Load<TextAsset>(shop_Table_DataPath));

        SortDeckList();
        SortShopList();
    }
}