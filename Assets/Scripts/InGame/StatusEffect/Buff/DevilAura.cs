using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DevilAura : StatusEffect, IWhileEffect, IAttackPowerRateEffect
{
    Transform _devilTransform;

    public int attackRate { get => _devilAuraPower; set => throw new System.NotImplementedException(); }

    private float _devilAuraRange;
    private int _devilAuraPower;

    public DevilAura(Battler battler, int duration, Transform devilTransform, float devilAuraRange, int devilAuraPower) : base(battler, duration)
    {
        Init(battler, duration);
        _originDuration = 0;
        effectType = EffectType.Buff;
        _devilAuraRange = devilAuraRange;
        _devilAuraPower = devilAuraPower;
        _devilTransform = devilTransform;
    }

    private float scanCoolTime = 0.3f;
    private float elapsedTime = 0.3f;

    public void UpdateEffectPower(float devilAuraRange, int devilAuraPower)
    {
        _devilAuraRange = devilAuraRange;
        _devilAuraPower = devilAuraPower;
    }

    public void WhileEffect()
    {
        if (elapsedTime < scanCoolTime)
        {
            elapsedTime += Time.deltaTime;
            return;
        }

        elapsedTime = 0;

        float dist = UtilHelper.CalCulateDistance(_battler.transform, _devilTransform);
        if (dist > _devilAuraRange)
        {
            _battler.RemoveStatusEffect(this);
            DeActiveEffect();
        }
    }
}
