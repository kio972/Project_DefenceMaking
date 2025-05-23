using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public interface IBlockingObject
{
    List<MonsterType> blockTargets { get; }
}

public class Barricade : Battler, IDestructableObjectKind, IBlockingObject
{
    private Tile _curTile;
    public Tile curTile { get => _curTile; }

    [SerializeField]
    private List<MonsterType> _blockTargets;
    public List<MonsterType> blockTargets { get => _blockTargets; }

    public override void Dead(Battler attacker)
    {
        base.Dead(attacker);
        _curTile.RemoveObject();
        Destroy(gameObject);
    }

    public void DestroyObject()
    {
        Dead(null);
    }

    public void SetTileInfo(Tile tile) => _curTile = tile;

    public override void Init()
    {
        base.Init();
        int index = UtilHelper.Find_Data_Index(battlerID, DataManager.Instance.battler_Table, "id");
        if (index != -1)
        {
            InitStats(index);
        }
        _curTile.SetObject(this);
    }
}
