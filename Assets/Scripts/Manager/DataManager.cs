using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : Singleton<DataManager>
{
    private List<Dictionary<string, object>> wave_Table;
    private List<Dictionary<string, object>> deckList;
    private List<Dictionary<string, object>> trap_Table;
    private List<Dictionary<string, object>> monster_Table;
    private List<Dictionary<string, object>> adventurer_Table;

    public List<Dictionary<string, object>> Wave_Table { get => wave_Table; }
    public List<Dictionary<string, object>> Deck_Table { get => deckList; }
    public List<Dictionary<string, object>> Trap_Table { get => trap_Table; }
    public List<Dictionary<string, object>> Monster_Table { get => monster_Table; }
    public List<Dictionary<string, object>> Adventurer_Table { get => adventurer_Table; }

    // csv파일 주소(Resource폴더 내)
    private string wave_Table_DataPath = "Data/waveData";
    private string deckList_DataPath = "Data/deckList";
    private string trap_Table_DataPath = "Data/trapData";
    private string monster_Table_DataPath = "Data/monsterTable";
    private string adventurer_Table_DataPath = "Data/adventurerTable";

    private void Init()
    {
        // csv파일 불러오는 함수
        wave_Table = CSVLoader.LoadCSV(Resources.Load<TextAsset>(wave_Table_DataPath));
        deckList = CSVLoader.LoadCSV(Resources.Load<TextAsset>(deckList_DataPath));
        trap_Table = CSVLoader.LoadCSV(Resources.Load<TextAsset>(trap_Table_DataPath));
        monster_Table = CSVLoader.LoadCSV(Resources.Load<TextAsset>(monster_Table_DataPath));
        adventurer_Table = CSVLoader.LoadCSV(Resources.Load<TextAsset>(adventurer_Table_DataPath));
    }
}