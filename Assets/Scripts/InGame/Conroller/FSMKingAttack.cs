using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSMKingAttack : FSMSingleton<FSMKingAttack>, CharState<Battler>
{
    /*excute : 마왕이 0.5f거리이내에 들어올때까지 이동수행
    out case
    1.현재 사거리 안에 마왕 외의 적이 있을경우 -> Attack으로, curTarget : 해당 적
    2.마왕과 0.5f거리 이내에 들어왔을경우 -> 마왕에게 공격 수행 후 Dead로 */

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
