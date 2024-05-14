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

    public List<Dictionary<string, object>> Wave_Table { get => wave_Table; }
    public List<Dictionary<string, object>> Deck_Table { get => deckList; }
    public List<Dictionary<string, object>> Battler_Table { get => battler_Table; }
    public List<Dictionary<string, object>> TimeRate_Table { get => timeRate_Table; }
    public List<Dictionary<string, object>> Language_Table { get => language_Table; }
    public List<Dictionary<string, object>> Research_Table { get => research_Table; }
    public List<Dictionary<string, object>> ScriptsMalpongsun_Table { get => scriptsMalpongsun_Table; }
    public List<Dictionary<string, object>> Quest_Table { get => quest_Table; }
    public List<Dictionary<string, object>> QuestMessage_Table { get => questMessage_Table; }


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

    public Dictionary<string, object> GetMalpoongsunScript(string id)
    {
        int index = UtilHelper.Find_Data_Index(id, scriptsMalpongsun_Table, "id");
        if (index == -1)
            return null;

        return scriptsMalpongsun_Table[index];
    }

    // csv���� �ּ�(Resource���� ��)
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

    private void Init()
    {
        // csv���� �ҷ����� �Լ�
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
    }
}