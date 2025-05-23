using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using UniRx;
using System.Linq;

public class KingSlime : Monster, ISpawnTimeModifier
{
    //스킬1. 슬라임의 분열 추가 회수 +1
    //스킬2. 모든 슬라임 생산 속도 50% 감소
    //스킬3. 3~7마리의 슬라임을 슬라임 킹 타일에 소환

    private CancellationTokenSource source;

    private float skillCoolTime = 5760;
    private float curSkillCoolTime = 0;

    readonly string[] targetSlimes = { "slime_mucus", "slime_poison", "slime_explosion" };

    public float spawnTimeWeight { get => 0.5f; }

    private int additionalSplitCount = 1;

    [SerializeField]
    GameObject kingAttackEffect;

    public override string GetStat(StatType statType)
    {
        if(statType == StatType.BuffTexts)
        {
            int value = Mathf.RoundToInt((1 / spawnTimeWeight - 1) * 100);
            return $"{DataManager.Instance.GetDescription("tooltip_buff_4").Replace("$", additionalSplitCount.ToString())}\n{DataManager.Instance.GetDescription("tooltip_buff_5").Replace("$", value.ToString())}";
        }

        return base.GetStat(statType);
    }

    public override void Attack()
    {
        base.Attack();
        if (kingAttackEffect != null)
            EffectPooling.Instance.PlayEffect(kingAttackEffect, transform);
    }

    private void ModifySpawnerCoolTime(MonsterSpawner spawner, bool value)
    {
        if (!targetSlimes.Contains(spawner._TargetName))
            return;

        spawner.SetSpawntimeModifier(this, value);
    }

    private void ModifySlimeSplitCount(Monster monster, int value)
    {
        if(monster is Slime slime)
            slime.ModifySplitCount(value);
    }

    private void ExcuteSpawnRandomSlime()
    {
        int randomIndex = Random.Range(0, targetSlimes.Length);
        BattlerPooling.Instance.SpawnMonster(targetSlimes[randomIndex], _curNode);
    }

    private async UniTaskVoid SpawnSlimes()
    {
        while(isDead)
        {
            await UniTask.Yield(source.Token);
            if (isCollapsed)
                continue;

            curSkillCoolTime += GameManager.Instance.InGameDeltaTime;

            if(curSkillCoolTime > skillCoolTime)
            {
                curSkillCoolTime = 0;
                int randomCount = Random.Range(3, 8); //3~7마리 소환 (30분 간격으로)
                for(int i = 0; i < randomCount; i++)
                {
                    ExcuteSpawnRandomSlime();
                    float waitTime = 0;
                    while(waitTime < 30)
                    {
                        waitTime += GameManager.Instance.InGameDeltaTime;
                        await UniTask.Yield(source.Token);
                    }
                }
            }
        }
    }

    public override void Dead(Battler attacker)
    {
        source.Cancel();
        source.Dispose();

        DeactiveSkill();
        curNode.curTile.spawnLock = false;
        base.Dead(attacker);
    }

    public override void Init()
    {
        base.Init();

        source = new CancellationTokenSource();
        _isCollapsed.Value = true;


        _isCollapsed.Subscribe(isCollapsed =>
        {
            if (isCollapsed)
                DeactiveSkill();
            else
                ActiveSkill();
        }).AddTo(source.Token);

        GameManager.Instance.monsterList.ObserveAdd().Where(_ => !isCollapsed).Subscribe(_ => ModifySlimeSplitCount(_.Value, 1)).AddTo(source.Token);
        GameManager.Instance.monsterSpawner.ObserveAdd().Where(_ => !isCollapsed).Subscribe(_ => ModifySpawnerCoolTime(_.Value, true)).AddTo(source.Token);
        SpawnSlimes().Forget();
    }

    private void ActiveSkill()
    {
        foreach (var monster in GameManager.Instance.monsterList)
            ModifySlimeSplitCount(monster, additionalSplitCount);

        foreach (var spawner in GameManager.Instance.monsterSpawner)
            ModifySpawnerCoolTime(spawner, true);
    }

    private void DeactiveSkill()
    {
        foreach (var monster in GameManager.Instance.monsterList)
            ModifySlimeSplitCount(monster, -additionalSplitCount);

        foreach (var spawner in GameManager.Instance.monsterSpawner)
            ModifySpawnerCoolTime(spawner, false);
    }

    public override void Patrol()
    {
        //순찰을 실행하지않음
    }

    private void OnDestroy()
    {
        source?.Cancel();
        source?.Dispose();
    }
}
