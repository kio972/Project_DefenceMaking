using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class AdventurerBoss : Adventurer
{
    private int adventurerIndex = -1;

    private float skillTime = 36f;
    private float ccTime = 60f;

    private bool isUsingSkill = false;
    private float curParryCoolTime = 0f;
    private float parryCoolTime = 300f;

    public override void Attack()
    {
        if (curTarget == null || curTarget.isDead)
            return;

        base.Attack();

        //���� �� ���� ��� �� �������� ȹ��
        if(curTarget.isDead)
        {
            AddStatusEffect<Rage>(new Rage(this, 120));
        }
    }

    public override void UseSkill()
    {
        curParryCoolTime = parryCoolTime;
        ExcuteParry().Forget();
    }

    private async UniTaskVoid ExcuteParry()
    {
        isUsingSkill = true;
        animator.PlayAnimation("Skill");

        float elpasedTime = 0f;
        while(elpasedTime < skillTime)
        {
            elpasedTime += GameManager.Instance.InGameDeltaTime;
            if (isDead)
                break;
            await UniTask.WaitForSeconds(GameManager.Instance.InGameDeltaTime, cancellationToken: gameObject.GetCancellationTokenOnDestroy());
        }

        isUsingSkill = false;
        if(!isDead)
            ChangeState(FSMPatrol.Instance);
    }

    public override void GetTureDamage(int damage)
    {
        //����������(�����̻�) �� ���� ������ 2��ó��
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

    //��� �̺�Ʈ�� ���丮�ʿ��� ó������
    //public override void Dead(Battler attacker)
    //{
    //    base.Dead(attacker);
    //    
    //}

    public override void Init()
    {
        base.Init();
        curParryCoolTime = 0;
    }

    public override void Update()
    {
        base.Update();
        if (curParryCoolTime > 0f && !isUsingSkill)
            curParryCoolTime -= GameManager.Instance.InGameDeltaTime;
    }
}
