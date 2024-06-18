using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Golem : Monster
{
    public void UpdateHoldBack(int value)
    {
        holdBackCount += value;
    }

    public override void Init()
    {
        base.Init();
        holdBackCount += PassiveManager.Instance.golemHoldback_Weight;
    }
}
