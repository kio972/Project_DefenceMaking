using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackSpeedUpCondition : StatusEffect, IWhileEffect, IAttackSpeedEffect, IConditionEffect
{
    private System.Func<bool> _condition;
    public System.Func<bool> condition => _condition;
    private float _attackSpeedRate;
    public AttackSpeedUpCondition(Battler battler, int duration, float speedWeight, System.Func<bool> condition) : base(battler, duration)
    {
        Init(battler, duration);
        effectType = EffectType.Debuff;
        _attackSpeedRate = speedWeight;
        UpdateEffect(condition);
    }

    public float attackSpeedRate { get => _attackSpeedRate; set => _attackSpeedRate = value; }

    public void WhileEffect()
    {
        if (_condition.Invoke())
            return;

        _battler.RemoveStatusEffect(this);
        DeActiveEffect();
    }

    public void UpdateEffect(Func<bool> condition)
    {
        _condition = condition;
    }
}
