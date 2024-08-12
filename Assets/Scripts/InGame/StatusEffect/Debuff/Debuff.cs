using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DebuffType
{
    CC,
    DOT,
}

public class Debuff : StatusEffect
{
    public DebuffType debuffType { get; protected set; }

    public Debuff(Battler battler, int duration) : base(battler, duration)
    {
        Init(battler, duration);
    }
}
