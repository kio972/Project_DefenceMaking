using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICCEmmune
{
    HashSet<CCType> emmuneCCs { get; }
}

public class Golem : Monster, ICCEmmune
{
    private HashSet<CCType> _emmuneCCs = new HashSet<CCType>() { CCType.KnockBack };
    public HashSet<CCType> emmuneCCs { get => _emmuneCCs; }

    public void UpdateHoldBack(int value)
    {
        holdBackCount += value;
    }

    public override void Init()
    {
        base.Init();
        holdBackCount += PassiveManager.Instance.golemHoldback_Weight;
        cc_Emmunes.Add(CCType.KnockBack);
    }
}
