using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rage : StatusEffect, IMoveSpeedEffect, IAttackSpeedEffect, IStackable
{
    private float attackStackRate = 0.05f;
    private float moveStackRate = 0.05f;

    public float moveSpeedRate { get => _stackCount * moveStackRate; set => throw new System.NotImplementedException(); }
    public float attackSpeedRate { get => _stackCount * attackStackRate; set => throw new System.NotImplementedException(); }


    private int _stackCount;
    private int _maxStack = 10;
    public int stackCount { get => _stackCount; set => _stackCount = Mathf.Min(value, _maxStack); }

    public void StackEffect(float duration)
    {
        _duration.Value = duration;
    }

    public Rage(Battler battler, int duration) : base(battler, duration)
    {
        Init(battler, duration);
        effectType = EffectType.Buff;
    }
}