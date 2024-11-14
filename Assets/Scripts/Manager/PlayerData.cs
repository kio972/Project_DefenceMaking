using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

public struct QuestData
{
    public string id;
    public List<int> curVal;
    public float curTime;
}

public struct BattlerData
{
    public string id;
    public int curHp;
    public int maxHp;
    public float pos_x;
    public float pos_z;

    public int row;
    public int col;

    public int nextRow;
    public int nextCol;

    public int lastCrossedRow;
    public int lastCrossedCol;

    public List<int> crossedRow;
    public List<int> crossedCol;

    public Dictionary<string, object> additionalData;
}

public struct TileData
{
    public string id;
    public int row;
    public int col;
    public int rotation;
    public bool isDormant;
    public bool isRemovable;
    public string trapId;
    public int trapDuration;
}

public struct SpawnerData
{
    public int row;
    public int col;
    public string spawnerId;
    public float spawnerCool;
}

public struct ShopData
{
    public int id;
    public int curVal;
    public bool isSoldOut;
}

public class PlayerData
{
    //Scene 관련
    public string sceneName;

    //GameManager 관련
    public int curWave;
    public float curTime;
    public int gold;
    public int herb1;
    public int herb2;
    public int herb3;

    //타일 관련
    public List<TileData> tiles;
    public List<TileData> environments;
    public List<TileData> hiddenTiles;
    public int nextHiddenTileCount;

    public List<SpawnerData> spawners;

    //손패
    public List<int> cardIdes;
    public List<int> deckLists;

    //전투 현황
    public List<BattlerData> enemys;
    public List<BattlerData> allies;
    public BattlerData devil;

    //연구 현황
    public List<string> researchIdes;
    public string curResearch;
    public float curResearchTime;

    //상점 현황
    public List<ShopData> herbData;
    public List<ShopData> itemsData;
    public int itemFluctCool;

    //퀘스트 현황
    public List<string> clearedQuests;
    public List<string> enqueuedQuests;
    public List<QuestData> curQuests;
}