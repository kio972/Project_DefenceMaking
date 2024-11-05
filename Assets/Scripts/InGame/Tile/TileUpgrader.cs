using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileUpgrader : MonoBehaviour, ISpeedModify
{
    public UnitType targetUnit { get => UnitType.Player; }
    public float speedRate { get => 1.2f; }

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

    [SerializeField]
    private Material _upgradeMaterial;

    public void UpgradeTile()
    {
        if (tile == null || tile.isUpgraded)
            return;

        Renderer renderer = transform.GetChild(0)?.GetComponent<Renderer>();
        if (renderer != null)
            renderer.material = _upgradeMaterial;

        tile.isUpgraded = true;
        if (tile._TileType == TileType.Path)
            tile.AddAllySpeedMult(this);
    }
}
