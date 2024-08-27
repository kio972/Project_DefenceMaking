using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSMPatrol : FSMSingleton<FSMPatrol>, CharState<Battler>
{
    /*excute : ���չ��� ���� Ž������
    out case
    0. hp�� 0���Ϸ� ����������� -> Dead��
    1.����Ÿ���� ��Ÿ��
    1.1 ��Ÿ� �ȿ� ���� ���� ��� -> Attack����, curTarget�� �� ����
    1.2 ���� ������ ������ ���ݹ��� ��� -> Chase��, curTarget�� �� ����
    2.����Ÿ���� ��Ÿ�� -> Room
    3.����Ÿ���� ���չ� -> KingAttack */

    private bool NeedChange(Battler e)
    {
        //0
        if (e.curHp <= 0)
        {
            e.ChangeState(FSMDead.Instance);
            return true;
        }

        //2
        if(e.CurTile.curTile._TileType == TileType.Room && e.CurTile.curTile.IsBigRoom)
        {
            e.ChangeState(FSMRoom.Instance);
            return true;
        }
        //3
        else if (e.CurTile.curTile._TileType == TileType.End)
        {
            e.ChangeState(FSMKingAttack.Instance);
            return true;
        }

        return false;
    }

    private bool AttackCheck(Battler e)
    {
        //1.1
        Battler curTarget = e.BattleCheck();
        if (curTarget != null)
        {
            e.curTarget = curTarget;
            e.ChangeState(FSMAttack.Instance);
            return true;
        }

        //1.2
        if(e.chaseTarget != null)
        {
            e.ChangeState(FSMChase.Instance);
            return true;
        }
        return false;
    }

    public void Enter(Battler e)
    {
        
    }

    public void Excute(Battler e)
    {
        if (NeedChange(e))
            return;

        if (AttackCheck(e))
            return;

        e.Patrol();
    }

    public void Exit(Battler e)
    {
        
    }
}
