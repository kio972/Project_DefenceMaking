using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManaStone : Environment, IManaSupply
{
    public int manaValue { get => (int)value; }

    protected override void CustomFunc()
    {
        //PassiveManager.Instance.AddManaStone(_CurNode, (int)value);
    }
}
