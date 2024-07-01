using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Linq;
using UniRx;
using UniRx.Triggers;

public class MimicRecover : MimicHider
{
    private float healTimer = 0f;
    private readonly float healTime = 60;

    public override void Init()
    {
        base.Init();
        curSeduceCount = 0;
    }

    private void CheckHealing()
    {
        if((object)CurState != FSMHide.Instance || curHp == maxHp)
        {
            healTimer = 0f;
            return;
        }

        healTimer += GameManager.Instance.InGameDeltaTime;
        if(healTimer >= healTime)
        {
            healTimer = 0f;
            int healValue = Mathf.CeilToInt(maxHp / 5);
            GetHeal(healValue, this);
        }
    }

    public override void Update()
    {
        base.Update();
        CheckHealing();
    }
}
