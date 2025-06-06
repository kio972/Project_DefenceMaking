using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSMPatrol : FSMSingleton<FSMPatrol>, CharState<Battler>
{
    /*excute : 마왕방을 향해 탐색진행
    out case
    0. hp가 0이하로 떨어졌을경우 -> Dead로
    1.현재타일이 길타일
    1.1 사거리 안에 적이 들어온 경우 -> Attack으로, curTarget에 적 저장
    1.2 근접 형태의 적에게 공격받은 경우 -> Chase로, curTarget에 적 저장
    2.현재타일이 방타일 -> Room
    3.현재타일이 마왕방 -> KingAttack */

    private bool NeedChange(Battler e)
    {
        //0
        if (e.curHp <= 0)
        {
            e.ChangeState(FSMDead.Instance);
            return true;
        }

        //2
        if(e.curNode.curTile._TileType == TileType.Room && e.curNode.curTile.IsBigRoom)
        {
            e.ChangeState(FSMRoom.Instance);
            return true;
        }
        //3
        else if (e.curNode.curTile._TileType == TileType.End)
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
        e.UpdateMoveSpeed();
    }

    public void Exit(Battler e)
    {
        
    }
}
