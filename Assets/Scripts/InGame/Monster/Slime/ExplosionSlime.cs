using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionSlime : Slime
{
    [SerializeField] private int explosionDamage = 10;
    private void ExplodeEffect()
    {
        var targets = GetRangedTargets(transform.position, 1, false);
        foreach (Adventurer item in targets)
            item.GetDamage(explosionDamage, this);
    }

    public override void Dead(Battler attacker)
    {
        ExplodeEffect();
        base.Dead(attacker);
    }
}
