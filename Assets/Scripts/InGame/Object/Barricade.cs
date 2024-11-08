using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Barricade : Monster, IDestructableObjectKind
{
    private Tile _curTile;
    public Tile curTile { get => _curTile; }

    public void DestroyObject()
    {
        _curTile.RemoveObject();

        Dead(null);
    }

    public void SetTileInfo(Tile tile) => _curTile = tile;
}
