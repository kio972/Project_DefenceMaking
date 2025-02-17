using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

public class DevilAuraSkill : ISkill
{
    public bool IsPassive { get => true; }

    private float coolTime = 0.2f;
    private float curCoolTime = 0;

    private Battler king;

    public ReactiveProperty<float> coolRate { get; } = new ReactiveProperty<float>(0);

    public ReactiveProperty<bool> isReady { get; } = new ReactiveProperty<bool>(false);

    private float _devilAuraRange;
    private int _devilAuraPower;

    public bool UseSkill()
    {
        if (_devilAuraPower == 0 || _devilAuraRange == 0)
            return false;

        foreach (Monster monster in GameManager.Instance.monsterList)
        {
            if (monster.HaveEffect(out DevilAura aura))
            {
                aura.UpdateEffectPower(_devilAuraRange, _devilAuraPower);
                continue;
            }

            float dist = UtilHelper.CalCulateDistance(king.transform, monster.transform);
            if (dist > _devilAuraRange)
                continue;

            monster.AddStatusEffect<DevilAura>(new DevilAura(monster, 0, king.transform, _devilAuraRange, _devilAuraPower));
        }

        return true;
    }

    private async UniTaskVoid DevilAuraEffect()
    {
        while (!king.isDead)
        {
            await UniTask.Delay(System.TimeSpan.FromSeconds(coolTime), false, cancellationToken: king.GetCancellationTokenOnDestroy());

            UseSkill();
        }
    }

    public void SetDevilAuraValue(float devilAuraRange, int devilAuraPower)
    {
        _devilAuraRange = devilAuraRange;
        _devilAuraPower = devilAuraPower;
    }

    public void SkillInit()
    {
        king = GameManager.Instance.king;
        DevilAuraEffect().Forget();
    }

    public SkillData SaveSkill()
    {
        SkillData skillData = new SkillData();
        skillData.skillName = GetType().Name;
        skillData.curCool = curCoolTime;
        return skillData;
    }

    public void LoadSkill(SkillData data)
    {
        curCoolTime += data.curCool;
    }
}
