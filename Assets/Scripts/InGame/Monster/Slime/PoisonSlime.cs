using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoisonSlime : Slime
{
    [SerializeField]
    protected Transform middlePos;
    [SerializeField]
    GameObject explosionPrefab;

    private void PosionEffect()
    {
        var targets = GetRangedTargets(transform.position, 1, false);
        foreach(Adventurer item in targets)
            item.AddStatusEffect<Poison>(new Poison(item, 180));
        if (explosionPrefab != null)
            EffectPooling.Instance.PlayEffect(explosionPrefab, middlePos);
    }

    public override void Dead(Battler attacker)
    {
        PosionEffect();
        base.Dead(attacker);
    }
}
