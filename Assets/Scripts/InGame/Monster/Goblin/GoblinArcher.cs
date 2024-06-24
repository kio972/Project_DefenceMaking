using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoblinArcher : Goblin
{
    public override void Attack()
    {
        base.Attack();
        if (curTarget != null && !curTarget.isDead)
            curTarget.AddStatusEffect<Poison>(new Poison(curTarget, 180));
    }
}
