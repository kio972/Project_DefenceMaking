using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionSlime : Slime
{
    [SerializeField] private int explosionDamage = 10;

    [SerializeField]
    protected Transform middlePos;
    [SerializeField]
    GameObject explosionPrefab;

    private void ExplodeEffect()
    {
        var targets = GetRangedTargets(transform.position, 1, false);
        foreach (Adventurer item in targets)
            item.GetDamage(explosionDamage, this);
        if (explosionPrefab != null)
            EffectPooling.Instance.PlayEffect(explosionPrefab, middlePos);
    }

    public override void Dead(Battler attacker)
    {
        ExplodeEffect();
        base.Dead(attacker);
    }
}
