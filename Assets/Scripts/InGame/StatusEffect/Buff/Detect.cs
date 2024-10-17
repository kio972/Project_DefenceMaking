using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Detect : StatusEffect
{
    public Detect(Battler battler, int duration) : base(battler, duration)
    {
        Init(battler, duration);
        effectType = EffectType.Buff;
    }
}