using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GolemObsidian : GolemIron
{
    [SerializeField]
    GameObject explosionPrefab;

    ParticleSystem _explosionEffect;
    ParticleSystem explosionEffect
    {
        get
        {
            if(_explosionEffect == null)
                _explosionEffect = Instantiate(explosionPrefab, middlePos)?.GetComponentInChildren<ParticleSystem>();
            return _explosionEffect;
        }
    }

    protected virtual void ShieldExplosion()
    {
        int explosionDamage = Mathf.FloorToInt(maxHp * 0.2f * 0.5f);
        List<Battler> targets = GetRangedTargets(transform.position, 1f, false);
        foreach (var target in targets)
            target.GetDamage(explosionDamage, this);

        if(explosionEffect != null)
        {
            explosionEffect.gameObject.SetActive(true);
            explosionEffect.Play();
        }
    }

    public override void GetDamage(int damage, Battler attacker)
    {
        bool haveSheild = shield > 0;
        base.GetDamage(damage, attacker);
        if (haveSheild && shield <= 0)
            ShieldExplosion();
    }
}
