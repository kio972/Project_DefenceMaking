using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSMKingAttack : FSMSingleton<FSMKingAttack>, CharState<Battler>
{
    /*excute : ������ 0.5f�Ÿ��̳��� ���ö����� �̵�����
    out case
    1.���� ��Ÿ� �ȿ� ���� ���� ���� ������� -> Attack����, curTarget : �ش� ��
    2.���հ� 0.5f�Ÿ� �̳��� ��������� -> ���տ��� ���� ���� �� Dead�� */

    private bool AttackCheck(Battler e)
    {
        //1.1
        Battler curTarget = e.BattleCheck();
        if (curTarget != null)
        {
            e.ChangeState(FSMAttack.Instance);
            e.curTarget = curTarget;
            return true;
        }

        return false;
    }

    private bool NeedChange(Battler e)
    {
        if (e.curHp <= 0)
        {
            e.ChangeState(FSMDead.Instance);
            return true;
        }


        return false;
    }

    public void Enter(Battler e)
    {
        GameManager.Instance.adventurer_entered_BossRoom.Add(e);
    }

    public void Excute(Battler e)
    {
        if (NeedChange(e))
            return;

        if (AttackCheck(e))
            return;

        if(Vector3.Distance(GameManager.Instance.king.transform.position, e.transform.position) < 0.5f)
        {
            e.curTarget = GameManager.Instance.king;
            e.Play_AttackAnimation();
            return;
        }

        e.transform.position = Vector3.MoveTowards(e.transform.position, GameManager.Instance.king.transform.position, e.curMoveSpeed * Time.deltaTime * GameManager.Instance.timeScale);
    }

    public void Exit(Battler e)
    {
        GameManager.Instance.adventurer_entered_BossRoom.Remove(e);
    }
}
