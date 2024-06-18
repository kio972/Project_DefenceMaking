using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DevilAura : StatusEffect, IWhileEffect, IAttackPowerEffect
{
    Transform devilTransform;

    public int attackPower { get => PassiveManager.Instance.devilAuraPower; set => throw new System.NotImplementedException(); }

    public DevilAura(Battler battler, int duration, Transform devilTransform) : base(battler, duration)
    {
        Init(battler, duration);
        _originDuration = 0;
        effectType = EffectType.Buff;
    }

    public void WhileEffect()
    {
        float dist = UtilHelper.CalCulateDistance(_battler.transform, devilTransform);
        if (dist > PassiveManager.Instance.devilAuraRange)
        {
            _battler.RemoveStatusEffect(this);
            DeActiveEffect();
        }
    }
}
