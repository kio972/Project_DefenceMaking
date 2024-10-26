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

public interface IToolTipEffect
{
    void ShowEffect(bool value);
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

    public string toolTipKey_header;
    public string toolTipKey_descs;
}