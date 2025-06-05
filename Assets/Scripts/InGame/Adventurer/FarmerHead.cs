using System.Collections;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;
using Cysharp.Threading;
using UnityEngine;
using System.Threading;

public class FarmerHead : Adventurer
{
    private readonly float _skillCoolTime = 720;
    private float _curSkillCoolTime = 0;

    private bool _isSkill = false;

    private readonly int skillDamage = 30;
    private readonly int skillRange = 1;
    private readonly int knockBackTile = 1;

    private CompositeDisposable disposable;

    public override void Play_AttackAnimation()
    {
        if (animator == null)
            return;

        _isSkill = _curSkillCoolTime <= 0;
        if(_isSkill)
            _curSkillCoolTime = _skillCoolTime;
        animator.UseSkill(_isSkill);
        base.Play_AttackAnimation();
    }

    public void SkillAttack()
    {
        //if (attackEffect != null)
        //    EffectPooling.Instance.PlayEffect(attackEffect, curTarget.transform, Vector3.zero, 0.9f);

        List<Battler> targets = GetRangedTargets(transform.position, 1);
        foreach (Battler battler in targets)
        {
            int tempDamage = TempDamage(skillDamage);
            battler.GetDamage(tempDamage, this);


            Direction knockBackDirection = UtilHelper.CheckClosestDirection(battler.transform.position - transform.position);
            battler.KnockBack(this, knockBackDirection, 1, 30);
        }
    }

    public override void Attack()
    {
        if (_isSkill)
            SkillAttack();
        else
            base.Attack();
    }

    public override void Dead(Battler attacker)
    {
        base.Dead(attacker);
        disposable.Dispose();
    }

    public override void Init()
    {
        base.Init();

        disposable = new CompositeDisposable();
        _curSkillCoolTime = _skillCoolTime;
        var coolTimeStream = Observable.EveryUpdate().Where(_ => _curSkillCoolTime > 0)
            .Subscribe(_ =>
            {
                _curSkillCoolTime -= GameManager.Instance.InGameDeltaTime;
                if (_curSkillCoolTime < 0)
                    _curSkillCoolTime = 0;
            }).AddTo(disposable);
    }
}
