using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Swamp : Environment, ISpeedModify
{
    private UnitType _targetUnit = UnitType.Enemy;
    public UnitType targetUnit { get => _targetUnit; }
    public float speedRate { get => value; }

    protected override void CustomFunc()
    {

    }
}
