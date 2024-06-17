using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Seduce : Debuff, IEnterEffect, IWhileEffect, IExitEffect
{
    private Battler _attacker;

    public Seduce(Battler battler, int duration, Battler attacker) : base(battler, duration)
    {
        Init(battler, duration);
        effectType = EffectType.Debuff;
        debuffType = DebuffType.CC;
        _attacker = attacker;
    }

    public void EnterEffect()
    {
        _battler.chaseTarget = _attacker;
        _battler.ChangeState(FSMChase.Instance);
    }

    public void ExitEffect()
    {
        _battler.ChangeState(FSMPatrol.Instance);
        _battler.RemoveStatusEffect(this);
        DeActiveEffect();
    }

    public void WhileEffect()
    {
        if (_attacker == null || _attacker.isDead)
        {
            ExitEffect();
            return;
        }

        if((object)_battler.CurState == FSMAttack.Instance && _battler.curTarget == _attacker)
        {
            ExitEffect();
            return;
        }

        _battler.chaseTarget = _attacker;
    }
}
