using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DevilAura : StatusEffect, IWhileEffect, IAttackPowerRateEffect
{
    Transform devilTransform;

    public int attackRate { get => PassiveManager.Instance.devilAuraPower; set => throw new System.NotImplementedException(); }

    public DevilAura(Battler battler, int duration, Transform devilTransform) : base(battler, duration)
    {
        Init(battler, duration);
        _originDuration = 0;
        effectType = EffectType.Buff;
        this.devilTransform = devilTransform;
    }

    private float scanCoolTime = 0.3f;
    private float elapsedTime = 0.3f;

    public void WhileEffect()
    {
        if (elapsedTime < scanCoolTime)
        {
            elapsedTime += Time.deltaTime;
            return;
        }

        elapsedTime = 0;

        float dist = UtilHelper.CalCulateDistance(_battler.transform, devilTransform);
        if (dist > PassiveManager.Instance.devilAuraRange)
        {
            _battler.RemoveStatusEffect(this);
            DeActiveEffect();
        }
    }
}
