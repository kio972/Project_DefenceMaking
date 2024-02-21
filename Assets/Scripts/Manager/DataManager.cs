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

    public List<Dictionary<string, object>> Wave_Table { get => wave_Table; }
    public List<Dictionary<string, object>> Deck_Table { get => deckList; }
    public List<Dictionary<string, object>> Battler_Table { get => battler_Table; }
    public List<Dictionary<string, object>> TimeRate_Table { get => timeRate_Table; }
    public List<Dictionary<string, object>> Language_Table { get => language_Table; }
    public List<Dictionary<string, object>> Research_Table { get => research_Table; }

    private List<int> tileCard_Indexs;
    private List<int> monsterCard_Indexs;
    private List<int> trapCard_Indexs;
    private List<int> environmentCard_Indexs;

    private List<int> pathCard_Indexs;
    private List<int> roomCard_Indexs;
    private List<int> roomPartCard_Index;
    private List<int> roomTypeCard_Indexs;
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
                    string adventurerName = data["adventure"].ToString();
                    int number = Convert.ToInt32(data["num"]);
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
                roomTypeCard_Indexs = new List<int>(RoomCard_Indexs);
                foreach (int val in RoomPartCard_Indexs)
                    roomTypeCard_Indexs.Add(val);
            }

            return roomTypeCard_Indexs;
        }
    }

    public List<int> TileCard_Indexs
    {
        get
        {
            if(tileCard_Indexs == null)
                tileCard_Indexs = Find_Typeof_Index(deckList, "cardtype", "tile");
            return tileCard_Indexs;
        }
    }
    public List<int> PathCard_Indexs
    {
        get
        {
            if (pathCard_Indexs == null)
                pathCard_Indexs = Find_Typeof_Index(deckList, "type", "road");
            return pathCard_Indexs;
        }
    }
    public List<int> RoomCard_Indexs
    {
        get
        {
            if (roomCard_Indexs == null)
                roomCard_Indexs = Find_Typeof_Index(deckList, "type", "room");
            return roomCard_Indexs;
        }
    }

    public List<int> RoomPartCard_Indexs
    {
        get
        {
            if (roomPartCard_Index == null)
                roomPartCard_Index = Find_Typeof_Index(deckList, "type", "roomPart");
            return roomPartCard_Index;
        }
    }

    public List<int> EnvironmentCard_Indexs
    {
        get
        {
            if (environmentCard_Indexs == null)
                environmentCard_Indexs = Find_Typeof_Index(deckList, "type", "environment");
            return environmentCard_Indexs;
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

    public List<int> MonsterCard_Indexs
    {
        get
        {
            if (monsterCard_Indexs == null)
                monsterCard_Indexs = Find_Typeof_Index(deckList, "cardtype", "monster");
            return monsterCard_Indexs;
        }
    }
    public List<int> TrapCard_Indexs
    {
        get
        {
            if (trapCard_Indexs == null)
                trapCard_Indexs = Find_Typeof_Index(deckList, "cardtype", "trap");
            return trapCard_Indexs;
        }
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

    // csv파일 주소(Resource폴더 내)
    private string wave_Table_DataPath = "Data/waveData";
    private string deckList_DataPath = "Data/deckList";
    private string battler_Table_DataPath = "Data/battlerTable";
    private string timeRate_Table_DataPath = "Data/timeData";
    private string language_Table_DataPath = "Data/languageData";
    private string scripts_Table_DataPath = "Data/scriptData";
    private string buff_Table_DataPath = "Data/buffData";
    private string research_Table_DataPath = "Data/researchData";

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
    }
}