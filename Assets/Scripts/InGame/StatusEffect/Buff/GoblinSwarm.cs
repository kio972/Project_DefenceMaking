using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoblinSwarm : StatusEffect, IWhileEffect, IAttackPowerEffect, IAttackSpeedEffect, IStackable
{
    private int number = 0;
    private float _buffDist = 0;

    private int[] _attackDamage = { 0, 1, 2, 3, 4, 5 };
    private float[] _attackSpeed = { 1, 1.2f, 1.4f, 1.6f, 1.8f, 2.0f };

    public int attackDamage { get => _attackDamage[number]; set => throw new System.NotImplementedException(); }
    public float attackSpeedRate { get => _attackSpeed[number]; set => throw new System.NotImplementedException(); }


    private int _stackCount;
    private int _maxStack = 5;
    public int stackCount { get => _stackCount; set => _stackCount = Mathf.Min(value, _maxStack); }

    public void StackEffect(float duration)
    {

    }

    public GoblinSwarm(Battler battler, int duration, float buffDist) : base(battler, duration)
    {
        Init(battler, duration);
        _originDuration = 0;
        _buffDist = buffDist;
        effectType = EffectType.Buff;
    }

    private float scanCoolTime = 0.3f;
    private float elapsedTime = 0.3f;

    public void WhileEffect()
    {
        if(elapsedTime < scanCoolTime)
        {
            elapsedTime += Time.deltaTime;
            return;
        }

        elapsedTime = 0;
        int targetCount = 0;
        foreach (Monster goblin in GameManager.Instance.monsterList)
        {
            if (goblin is not Goblin || goblin == _battler)
                continue;

            float dist = UtilHelper.CalCulateDistance(_battler.transform, goblin.transform);
            if (dist > _buffDist)
                continue;
            targetCount++;
        }

        number = Mathf.Min(targetCount, _attackDamage.Length - 1);
        stackCount = number;
        if (number == 0)
        {
            _battler.RemoveStatusEffect(this);
            DeActiveEffect();
        }
    }
}