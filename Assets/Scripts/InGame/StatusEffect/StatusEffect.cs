using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using System.Threading;
using Cysharp.Threading.Tasks;

public enum EffectType
{
    Buff,
    Debuff,
}

public interface IAttackSpeedEffect
{
    public int attackSpeedRate { get; set; }
}

public interface IAttackPowerEffect
{
    public int attackDamage { get; set; }
}

public interface IAttackPowerRateEffect
{
    public int attackRate { get; set; }
}

public interface IEnterEffect
{
    public void EnterEffect();
}

public interface IExitEffect
{
    public void ExitEffect();
}

public interface IWhileEffect
{
    public void WhileEffect();
}

public interface IStackable
{
    public int stackCount { get; set; }
    public void StackEffect(float duration);
}

public class StatusEffect
{
    protected Battler _battler;
    public EffectType effectType { get; protected set; }
    public float _originDuration;
    public ReactiveProperty<float> _duration = new ReactiveProperty<float>();

    protected CancellationTokenSource _cancellationToken;

    public void UpdateEffect(float duration)
    {
        //스택이 가능하면 스택카운트 올리면서 이펙트발동
        if(this is IStackable stackable)
        {
            stackable.stackCount = stackable.stackCount + 1;
            stackable.StackEffect(duration);
            return;
        }

        //스택이 불가능하면 초기화
        Init(_battler, duration);
    }

    public async UniTaskVoid ExcuteEffect()
    {
        await UniTask.Yield();
        if (this is IStackable stackable)
            stackable.stackCount = 1;

        if (this is IEnterEffect enterEffect)
            enterEffect.EnterEffect();

        while(_duration.Value > 0 || _originDuration == 0)
        {
            if(_originDuration != 0)
                _duration.Value -= Time.deltaTime * GameManager.Instance.DefaultSpeed * GameManager.Instance.timeScale;
            if (this is IWhileEffect whileEffect)
                whileEffect.WhileEffect();

            await UniTask.Yield(_cancellationToken.Token);
        }

        if (this is IExitEffect exitEffect)
            exitEffect.ExitEffect();

        _battler.RemoveStatusEffect(this);
        DeActiveEffect();
    }

    public virtual void DeActiveEffect()
    {
        _cancellationToken?.Cancel();
    }

    protected void Init(Battler battler, float duration)
    {
        _battler = battler;
        _originDuration = duration;
        _duration.Value = duration;

        _cancellationToken?.Cancel();
        _cancellationToken = new CancellationTokenSource();
        ExcuteEffect().Forget();
    }

    public StatusEffect(Battler battler, float duration)
    {
        Init(battler, duration);
    }

    ~StatusEffect()
    {
        DeActiveEffect();
    }
}