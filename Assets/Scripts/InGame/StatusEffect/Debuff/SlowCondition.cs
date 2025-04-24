using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IConditionEffect
{
    System.Func<bool> condition { get; }
    void UpdateEffect(System.Func<bool> condition);
}

public class SpeedCondition : Debuff, IWhileEffect, IMoveSpeedEffect, IConditionEffect
{
    private System.Func<bool> _condition;
    public System.Func<bool> condition => _condition;
    private float _speedWeight;
    public SpeedCondition(Battler battler, int duration, float speedWeight, System.Func<bool> condition) : base(battler, duration)
    {
        Init(battler, duration);
        effectType = EffectType.Debuff;
        debuffType = DebuffType.CC;
        _speedWeight = speedWeight;
        UpdateEffect(condition);
    }

    public float moveSpeedRate { get => _speedWeight; set => _speedWeight = value; }

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

public class SlowCondition : SpeedCondition
{
    public SlowCondition(Battler battler, int duration, float speedWeight, Func<bool> condition) : base(battler, duration, speedWeight, condition)
    {
    }
}

public class FastCondition : SpeedCondition
{
    public FastCondition(Battler battler, int duration, float speedWeight, Func<bool> condition) : base(battler, duration, speedWeight, condition)
    {
    }
}