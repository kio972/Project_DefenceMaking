using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : Singleton<DataManager>
{
    private List<Dictionary<string, object>> wave_Table;
    private List<Dictionary<string, object>> deckList;
    private List<Dictionary<string, object>> trap_Table;

    public List<Dictionary<string, object>> Wave_Table { get => wave_Table; }
    public List<Dictionary<string, object>> Deck_Table { get => deckList; }
    public List<Dictionary<string, object>> Trap_Table { get => trap_Table; }


    // csv파일 주소(Resource폴더 내)
    private string wave_Table_DataPath = "Data/waveData";
    private string deckList_DataPath = "Data/deckList";
    private string trap_Table_DataPath = "Data/trapData";

    private void Init()
    {
        // csv파일 불러오는 함수
        //skill_Active_Dic = CSVLoader.LoadCSV(Resources.Load<TextAsset>(skill_Active_DataPath));
        wave_Table = CSVLoader.LoadCSV(Resources.Load<TextAsset>(wave_Table_DataPath));
        deckList = CSVLoader.LoadCSV(Resources.Load<TextAsset>(deckList_DataPath));
        trap_Table = CSVLoader.LoadCSV(Resources.Load<TextAsset>(trap_Table_DataPath));
    }
}