using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Poison : Debuff, IWhileEffect, IStackable
{
    private float tick = 0;

    public Poison(Battler battler, int duration) : base(battler, duration)
    {
        Init(battler, duration);
        effectType = EffectType.Debuff;
        debuffType = DebuffType.DOT;
        tick = 0;
    }

    private int _stackCount;
    private int _maxStack = 3;

    public int stackCount { get => _stackCount; set => _stackCount = Mathf.Min(value, _maxStack); }

    private readonly int[] _stackDamage = { 180, 300, 480 };

    public void StackEffect(float duration)
    {

    }

    public void WhileEffect()
    {
        tick += Time.deltaTime * GameManager.Instance.timeScale;
        if (tick < 60f)
            return;

        tick = 0f;
        _battler.GetDamage(_stackDamage[_stackCount], null);
    }
}
