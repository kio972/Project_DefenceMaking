using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompleteRoom
{
    public List<Tile> includeRooms;
    public int totalMana;
    public int spendedMana;

    public List<Tile> _IncludeRooms { get => includeRooms; }
    public int _RemainingMana { get { return totalMana - spendedMana; } }
    public Tile HeadRoom { get { return includeRooms[0]; } }

    public CompleteRoom(List<Tile> targetTiles, bool singleRoom = false)
    {
        includeRooms = targetTiles;
        totalMana = singleRoom ? 3 : includeRooms.Count;
        spendedMana = 0;
    }
}
