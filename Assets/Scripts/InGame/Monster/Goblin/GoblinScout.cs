using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoblinScout : Goblin
{
    public override void Attack()
    {
        base.Attack();
        if (curTarget != null && !curTarget.isDead)
            curTarget.RemoveStatusEffect<Cloaking>();
    }
}
