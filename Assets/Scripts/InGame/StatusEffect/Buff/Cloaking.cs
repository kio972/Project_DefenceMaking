using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cloaking : StatusEffect, IWhileEffect
{
    public Cloaking(Battler battler, int duration) : base(battler, duration)
    {
        Init(battler, duration);
        _originDuration = 0;
        effectType = EffectType.Buff;
    }

    public void WhileEffect()
    {
        
    }
}