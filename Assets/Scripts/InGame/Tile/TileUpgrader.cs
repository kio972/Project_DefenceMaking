using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileUpgrader : MonoBehaviour, IStatModifier, IBattlerEnterEffect
{
    public UnitType targetUnit { get => UnitType.Player; }
    public float modifyValue { get => 1.2f; }
    private Tile _tile;
    private Tile tile
    {
        get
        {
            if (_tile == null)
                _tile = GetComponent<Tile>();
            return _tile;
        }
    }

    //public StatType statType => StatType.MoveSpeed;

    private Material _originMaterial;
    [SerializeField]
    private Material _upgradeMaterial;

    public void Effect(Battler target)
    {
        if (target.unitType != UnitType.Player)
            return;

        TileNode curNode = target.curNode;
        target.AddStatusEffect<FastCondition>(new FastCondition(target, 0, modifyValue, () => curNode == target.curNode));
        if (GameManager.Instance.research.completedResearchs.Contains("r_t10002"))
        {
            target.AddStatusEffect<AttackSpeedUpCondition>(new AttackSpeedUpCondition(target, 0, modifyValue, () => curNode == target.curNode));
        }
    }

    public void UpgradeTile()
    {
        if (tile == null || tile.isUpgraded)
            return;

        Renderer renderer = transform.GetChild(0)?.GetComponent<Renderer>();
        if (renderer != null)
        {
            _originMaterial = renderer.material;
            renderer.material = _upgradeMaterial;
        }

        tile.isUpgraded = true;
        if (tile._TileType == TileType.Path)
        {
            tile.curTileEffects.Add(this);
            //tile.AddAllySpeedMult(this);
            //if (GameManager.Instance.research.completedResearchs.Contains("r_t10002"))
            //{
            //    tile.AddAllyAttackSpeedMult(this);
            //}
        }
        else if (tile._TileType is TileType.Room or TileType.Room_Single or TileType.Door)
        {
            int updateRoomMana = tile.RoomMana;
        }
    }

    public void DownGradeTile()
    {
        if (tile == null || !tile.isUpgraded)
            return;

        Renderer renderer = transform.GetChild(0)?.GetComponent<Renderer>();
        if (renderer != null)
            renderer.material = _originMaterial;

        tile.isUpgraded = false;
        //tile.RemoveAllyAttackSpeedMult(this);
        //tile.RemoveAllySpeedMult(this);
        //tile.RemoveEnemySpeedMult(this);
        tile.curTileEffects.Remove(this);
    }
}
