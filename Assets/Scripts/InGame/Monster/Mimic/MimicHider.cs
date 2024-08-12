using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Linq;
using UniRx;
using UniRx.Triggers;

public class MimicHider : Mimic
{
    private float hideTimer = 0f;
    private readonly float hideTime = 720f;

    public override void Patrol()
    {
        base.Patrol();
        hideTimer += GameManager.Instance.InGameDeltaTime;
        if(hideTimer >= hideTime)
            ChangeState(FSMHide.Instance);
    }

    public override void Init()
    {
        base.Init();
        curSeduceCount = 0;
    }

    public override void Update()
    {
        base.Update();
        if ((object)CurState != FSMPatrol.Instance)
            hideTimer = 0f;
    }
}
