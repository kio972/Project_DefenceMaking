using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThiefVeteran : Thief
{
    public override bool canAttackbyTrap { get => false; }

    public override void Init()
    {
        base.Init();
        RemoveStatusEffect<Stealth>();
        AddStatusEffect<Stealth_sup>(new Stealth_sup(this, 0));
    }
}
