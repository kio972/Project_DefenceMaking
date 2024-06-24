using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoisonSlime : Slime
{
    private void PosionEffect()
    {
        var targets = GetRangedTargets(transform.position, 1, false);
        foreach(Adventurer item in targets)
            item.AddStatusEffect<Poison>(new Poison(item, 180));
    }

    public override void Dead()
    {
        PosionEffect();
        base.Dead();
    }
}
