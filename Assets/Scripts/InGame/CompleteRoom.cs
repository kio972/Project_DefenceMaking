using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompleteRoom
{
    public List<Tile> includeRooms;
    public int totalMana;
    public int spendedMana;

    public CompleteRoom(List<Tile> targetTiles)
    {
        includeRooms = targetTiles;
        totalMana = 0;
        foreach (Tile tile in includeRooms)
            totalMana += tile.RoomMana;
        
        spendedMana = 0;
    }
}
