using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ToolTipType
{
    Devil,
    Special,
    Enemy,
    Ally,
    Trap,
    Spawner,
    Tile,
    Etc,
}

[RequireComponent(typeof(Collider))]
public class TooltipObject : MonoBehaviour
{
    [SerializeField]
    private ToolTipType _toolTipType;
    public ToolTipType toolTipType { get => _toolTipType; }

    [SerializeField]
    private int _subLevel = 0;
    public int subLevel { get => _subLevel; }
}