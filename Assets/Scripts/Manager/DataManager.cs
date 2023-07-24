using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : Singleton<DataManager>
{
    private List<Dictionary<string, object>> wave_Table;
    private List<Dictionary<string, object>> deckList;
    private List<Dictionary<string, object>> battler_Table;


    public List<Dictionary<string, object>> Wave_Table { get => wave_Table; }
    public List<Dictionary<string, object>> Deck_Table { get => deckList; }

    public List<Dictionary<string, object>> Battler_Table { get => battler_Table; }

    private List<int> tileCard_Indexs;
    private List<int> monsterCard_Indexs;
    private List<int> trapCard_Indexs;

    private List<int> pathCard_Indexs;
    private List<int> roomCard_Indexs;

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

    // csv파일 주소(Resource폴더 내)
    private string wave_Table_DataPath = "Data/waveData";
    private string deckList_DataPath = "Data/deckList";
    //private string trap_Table_DataPath = "Data/trapData";
    //private string monster_Table_DataPath = "Data/monsterTable";
    //private string adventurer_Table_DataPath = "Data/adventurerTable";
    private string battler_Table_DataPath = "Data/battlerTable";

    private void Init()
    {
        // csv파일 불러오는 함수
        wave_Table = CSVLoader.LoadCSV(Resources.Load<TextAsset>(wave_Table_DataPath));
        deckList = CSVLoader.LoadCSV(Resources.Load<TextAsset>(deckList_DataPath));
        //trap_Table = CSVLoader.LoadCSV(Resources.Load<TextAsset>(trap_Table_DataPath));
        //monster_Table = CSVLoader.LoadCSV(Resources.Load<TextAsset>(monster_Table_DataPath));
        //adventurer_Table = CSVLoader.LoadCSV(Resources.Load<TextAsset>(adventurer_Table_DataPath));
        battler_Table = CSVLoader.LoadCSV(Resources.Load<TextAsset>(battler_Table_DataPath));
    }
}