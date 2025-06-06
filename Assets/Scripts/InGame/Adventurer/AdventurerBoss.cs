using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Spine.Unity;
using UniRx;
using UnityEngine;

public class AdventurerBoss : Adventurer
{
    private int adventurerIndex = -1;

    private float skillTime = 36f;
    private float ccTime = 60f;

    private bool isUsingSkill = false;
    private float curParryCoolTime = 0f;
    private float parryCoolTime = 300f;

    private void PlayRageEffect(StatusEffect effect)
    {
        var skeletonAnimation = GetComponentInChildren<SkeletonAnimation>();
        if (skeletonAnimation == null) return;

        skeletonAnimation?.AnimationState.SetAnimation(1, "skill_b_stack", false);

        if (effect is Rage rage && rage.stackCount >= 10)
        {
            skeletonAnimation?.AnimationState.SetAnimation(2, "skill_b", true);
        }
    }

    public override void Attack()
    {
        if (curTarget == null || curTarget.isDead)
            return;

        base.Attack();

        //공격 후 적이 사망 시 광폭버프 획득
        if(curTarget!= null && curTarget.isDead)
        {
            var rage = AddStatusEffect<Rage>(new Rage(this, 120));
            PlayRageEffect(rage);
        }
    }

    public override void UseSkill()
    {
        if(isUsingSkill)
            return;
        curParryCoolTime = parryCoolTime;
        ExcuteParry().Forget();
    }

    private async UniTaskVoid ExcuteParry()
    {
        isUsingSkill = true;
        animator.PlayAnimation("skill_a");
        float animationTime = 1.333f;
        float elpasedTime = 0f;
        while(elpasedTime < skillTime)
        {
            elpasedTime += GameManager.Instance.InGameDeltaTime;
            animator.UpdateAttackSpeed((animationTime * GameManager.Instance.DefaultSpeed / skillTime) * GameManager.Instance.timeScale);
            if (isDead)
                break;
            await UniTask.Yield(cancellationToken: gameObject.GetCancellationTokenOnDestroy());
        }

        isUsingSkill = false;
        if(!isDead)
            ChangeState(FSMPatrol.Instance);
    }

    public override void GetTureDamage(int damage)
    {
        //고정데미지(상태이상) 에 대한 데미지 2배처리
        base.GetTureDamage(damage * 2);
    }

    private void ParryAttack(Battler target)
    {
        DamageTextPooling.Instance.TextEffect(transform.position, "MISS", 27f, Color.white, true);
        target.GetCC(this, ccTime);
    }

    public override void GetDamage(int damage, Battler attacker)
    {
        if(isUsingSkill)
        {
            ParryAttack(attacker);
            return;
        }

        if(curParryCoolTime <= 0f)
        {
            ChangeState(FSMSkill.Instance);
        }
        else
            base.GetDamage(damage, attacker);
    }

    //사망 이벤트는 스토리쪽에서 처리예정
    //public override void Dead(Battler attacker)
    //{
    //    base.Dead(attacker);
    //    
    //}

    public override void Init()
    {
        base.Init();
        curParryCoolTime = 0;

        _effects.ObserveRemove().Where(_ => _.Value is Rage).Subscribe(_ =>
        {
            var skeletonAnimation = GetComponentInChildren<SkeletonAnimation>();
            skeletonAnimation?.AnimationState.SetEmptyAnimation(2, 0.5f);
        }).AddTo(unitaskCancelTokenSource.Token);
    }

    public override void Update()
    {
        base.Update();
        if (curParryCoolTime > 0f && !isUsingSkill)
            curParryCoolTime -= GameManager.Instance.InGameDeltaTime;
    }
}
