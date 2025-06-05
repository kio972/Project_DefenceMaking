using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Linq;
using UniRx;
using UniRx.Triggers;
using System.Threading;
using System;

public class Mimic : Monster, IHide
{
    public bool canAttackbyTrap { get => true; }

    public virtual void HideAction()
    {
        Battler curTarget = BattleCheck();
        if (curTarget != null)
            ChangeState(FSMPatrol.Instance);
    }

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
        CheckNodeOut(_curNode).Forget();
        AddStatusEffect<Stealth>(new Stealth(this, 0));
        if (PassiveManager.Instance.isMimicBuffActive)
            attackTargetCount += 2;
        animator?.SetHide(true);
    }

    private async UniTaskVoid CheckNodeOut(TileNode curNode)
    {
        MonsterSpawner emptySpawner = new GameObject().AddComponent<MonsterSpawner>();
        emptySpawner.Init(curNode, _name, NodeManager.Instance.FindRoom(curNode.row, curNode.col));
        emptySpawner.isEmpty = true;
        await UniTask.WaitUntil(() => _curNode != curNode || isDead);
        emptySpawner.DestroyObject();
    }

    public override void LoadData(BattlerData data)
    {
        base.LoadData(data);
        curSeduceCount = System.Convert.ToInt32(data.additionalData["curSeduceCount"]);
        if (data.additionalData != null && data.additionalData.Count > 0)
        {
            bool hideState = System.Convert.ToBoolean(data.additionalData["hideState"]);
            if (!hideState)
            {
                animator?.SetHide(false);
                ChangeState(FSMPatrol.Instance);
            }
        }
    }

    public override BattlerData GetData()
    {
        BattlerData data = base.GetData();
        data.additionalData = new Dictionary<string, object>();
        data.additionalData.Add("curSeduceCount", curSeduceCount);
        data.additionalData.Add("hideState", (object)CurState == FSMHide.Instance);
        return base.GetData();
    }
}
