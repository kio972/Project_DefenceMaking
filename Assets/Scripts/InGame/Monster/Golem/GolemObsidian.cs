using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GolemObsidian : GolemIron
{
    private void ShieldExplosion()
    {
        int explosionDamage = Mathf.FloorToInt(maxHp * 0.2f * 0.5f);
        List<Battler> targets = GetRangedTargets(transform.position, 1f, false);
        foreach (var target in targets)
            target.GetDamage(explosionDamage, this);
    }

    public override void GetDamage(int damage, Battler attacker)
    {
        bool haveSheild = shield > 0;
        base.GetDamage(damage, attacker);
        if (haveSheild && shield <= 0)
            ShieldExplosion();
    }
}
