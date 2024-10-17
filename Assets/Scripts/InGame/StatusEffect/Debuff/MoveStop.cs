using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveStop : Debuff, IEnterEffect, IExitEffect
{
    public MoveStop(Battler battler, int duration) : base(battler, duration)
    {
        Init(battler, duration);
        effectType = EffectType.Debuff;
        debuffType = DebuffType.CC;
    }

    public void EnterEffect()
    {

    }

    public void ExitEffect()
    {

    }
}
