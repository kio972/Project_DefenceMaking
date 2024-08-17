using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoblinWarrior : Goblin
{
    float buffDist = 0.5f;
    private async UniTaskVoid GoblinSwarmEffect()
    {
        while (!isDead)
        {
            await UniTask.Delay(System.TimeSpan.FromSeconds(0.2f));
            if (HaveEffect<GoblinSwarm>())
                continue;

            foreach (Monster goblin in GameManager.Instance._MonsterList)
            {
                if (goblin == this || goblin is not Goblin)
                    continue;

                float dist = UtilHelper.CalCulateDistance(transform, goblin.transform);
                if (dist > buffDist)
                    continue;

                AddStatusEffect<GoblinSwarm>(new GoblinSwarm(this, 0, buffDist));
                break;
            }
        }
    }


    public override void Init()
    {
        base.Init();

        GoblinSwarmEffect().Forget();
    }
}
