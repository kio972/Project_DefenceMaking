using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

public struct TileData
{
    public int id { get; set; }
    public int row { get; set; }
    public int col { get; set; }
    public int rotation { get; set; }
}

public class PlayerData
{
    //GameManager 관련
    public int curWave;
    public float curTime;

    //타일 관련
    public List<TileData> tiles;
}