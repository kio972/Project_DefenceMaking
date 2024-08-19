using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoblinSwarm : StatusEffect, IWhileEffect, IAttackPowerEffect, IAttackSpeedEffect
{
    private int number = 0;
    private float _buffDist = 0;

    private int[] _attackDamage = { 0, 1, 2, 3, 4, 5 };
    private int[] _attackSpeed = { 0, 20, 40, 60, 80, 100 };

    public int attackDamage { get => _attackDamage[number]; set => throw new System.NotImplementedException(); }
    public int attackSpeedRate { get => _attackSpeed[number]; set => throw new System.NotImplementedException(); }

    public GoblinSwarm(Battler battler, int duration, float buffDist) : base(battler, duration)
    {
        Init(battler, duration);
        _originDuration = 0;
        _buffDist = buffDist;
        effectType = EffectType.Buff;
    }

    public void WhileEffect()
    {
        int targetCount = 0;
        foreach (Monster goblin in GameManager.Instance._MonsterList)
        {
            if (goblin is not Goblin || goblin == _battler)
                continue;

            float dist = UtilHelper.CalCulateDistance(_battler.transform, goblin.transform);
            if (dist > _buffDist)
                continue;
            targetCount++;
        }

        number = Mathf.Min(targetCount, _attackDamage.Length - 1);

        if(number == 0)
        {
            _battler.RemoveStatusEffect(this);
            DeActiveEffect();
        }
    }
}