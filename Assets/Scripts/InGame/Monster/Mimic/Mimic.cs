using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Linq;
using UniRx;
using UniRx.Triggers;

public class Mimic : Monster
{
    [SerializeField]
    private int seduceTargetCount = 1;
    protected int curSeduceCount = 0;

    protected CompositeDisposable disposables = new CompositeDisposable();

    private void Seduce()
    {
        System.Random random = new System.Random();
        var targets = GameManager.Instance.adventurersList.OrderBy(x => random.Next());
        foreach(var target in targets)
        {
            if (curSeduceCount >= seduceTargetCount)
                return;

            if (target.HaveEffect<Seduce>())
                continue;
            target.AddStatusEffect<Seduce>(new Seduce(target, 0, this));
            curSeduceCount++;
        }

        if (curSeduceCount >= seduceTargetCount)
            return;

        GameManager.Instance.adventurersList.ObserveAdd().Where(x => (object)this.CurState == FSMHide.Instance).Subscribe(x =>
        {
            x.Value.AddStatusEffect<Seduce>(new Seduce(x.Value, 0, this));
            curSeduceCount++;
            if (curSeduceCount >= seduceTargetCount)
                disposables.Dispose();
        }).AddTo(disposables);
    }

    public override void Init()
    {
        base.Init();
        curSeduceCount = 0;
        Seduce();
    }
}
