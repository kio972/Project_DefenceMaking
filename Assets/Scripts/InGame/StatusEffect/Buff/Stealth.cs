using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Stealth : StatusEffect, IEnterEffect, IWhileEffect
{
    public Stealth(Battler battler, int duration) : base(battler, duration)
    {
        Init(battler, duration);
        _originDuration = 0;
        effectType = EffectType.Buff;
        prevHp = battler.curHp;
    }

    private async UniTaskVoid ExitCheck()
    {
        await UniTask.WaitUntil(() => (object)_battler._CurState.Value != FSMHide.Instance, default, _cancellationToken.Token);

        _battler.RemoveStatusEffect(this);
        DeActiveEffect();
    }

    public void EnterEffect()
    {
        ExitCheck().Forget();
        _battler.ChangeState(FSMHide.Instance);
    }

    private int prevHp;

    public void WhileEffect()
    {
        int curHp = _battler.curHp;
        if (curHp < prevHp)
        {
            _battler.ChangeState(FSMPatrol.Instance);
            _battler.GetCC(_battler.curTarget, 0.5f * GameManager.Instance.DefaultSpeed);

            _battler.RemoveStatusEffect(this);
            DeActiveEffect();
        }

        prevHp = curHp;
    }
}