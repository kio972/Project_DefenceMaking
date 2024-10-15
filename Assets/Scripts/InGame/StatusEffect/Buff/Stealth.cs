using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stealth : StatusEffect, IEnterEffect
{
    public Stealth(Battler battler, int duration) : base(battler, duration)
    {
        Init(battler, duration);
        _originDuration = 0;
        effectType = EffectType.Buff;
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
    }
}