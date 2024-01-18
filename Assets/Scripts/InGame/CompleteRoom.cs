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
    public Tile HeadRoom { get { return CalculateMidTile(); } }

    private Tile CalculateMidTile()
    {
        if (includeRooms.Count == 1)
            return includeRooms[0];

        Vector3 vector = Vector3.zero;
        foreach (Tile tile in includeRooms)
            vector += tile.transform.position;

        vector = vector / includeRooms.Count;

        Tile resultTile = includeRooms[0];
        float minMagnitude = (vector - includeRooms[0].transform.position).magnitude;

        foreach(Tile tile in includeRooms)
        {
            float targetMagnitude = (vector - tile.transform.position).magnitude;
            if(targetMagnitude < minMagnitude)
            {
                minMagnitude = targetMagnitude;
                resultTile = tile;
            }
        }

        return resultTile;
    }

    public CompleteRoom(List<Tile> targetTiles, bool singleRoom = false)
    {
        includeRooms = targetTiles;
        totalMana = singleRoom ? 3 : includeRooms.Count;
        spendedMana = 0;
    }
}
