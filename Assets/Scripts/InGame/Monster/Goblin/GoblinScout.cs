using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoblinScout : Goblin
{
    protected override void RemoveOutCaseTargets(List<Battler> targets)
    {
        List<Battler> removeList = new List<Battler>();
        foreach (Battler battler in rangedTargets)
        {
            if (battler.isDead || !targets.Contains(battler))
                removeList.Add(battler);
        }

        foreach (Battler battler in removeList)
            rangedTargets.Remove(battler);
    }

    protected override Battler GetPriorityTarget()
    {
        Battler curTarget = null;
        bool isCloaked = false;
        foreach (Battler battle in rangedTargets)
        {
            if (curTarget == null) //사거리 내에 들어온 유일한 타겟일경우에만 지정가능하도록한다.
                curTarget = battle;
            else
            {
                if (!isCloaked && (object)battle.CurState == FSMHide.Instance)
                {
                    curTarget = battle;
                    continue;
                }

                if (isCloaked && (object)battle.CurState != FSMHide.Instance)
                    continue;

                bool isNearestTarget = Vector3.Distance(transform.position, battle.transform.position) < Vector3.Distance(transform.position, curTarget.transform.position);
                if (isNearestTarget && battle.tag != "King")
                    curTarget = battle;
            }
        }

        return curTarget;
    }
}
