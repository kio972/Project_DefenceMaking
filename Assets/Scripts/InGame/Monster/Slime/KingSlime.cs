using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using UniRx;
using System.Linq;

public class KingSlime : Monster
{
    //��ų1. �������� �п� �߰� ȸ�� +1
    //��ų2. ��� ������ ���� �ӵ� 50% ����
    //��ų3. 3~7������ �������� ������ ŷ Ÿ�Ͽ� ��ȯ

    private CancellationTokenSource source;

    private float skillCoolTime = 5760;
    private float curSkillCoolTime = 0;

    readonly string[] targetSlimes = { "slime_mucus", "slime_poison", "slime_explosion" };

    [SerializeField]
    GameObject kingAttackEffect;

    public override void Attack()
    {
        base.Attack();
        if (kingAttackEffect != null)
            EffectPooling.Instance.PlayEffect(kingAttackEffect, transform);
    }

    private void ModifySpawnerCoolTime(MonsterSpawner spawner, float value)
    {
        if (!targetSlimes.Contains(spawner._TargetName))
            return;

        spawner.ModifyCoolTime(value);
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
                int randomCount = Random.Range(3, 8); //3~7���� ��ȯ (30�� ��������)
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
        GameManager.Instance.monsterSpawner.ObserveAdd().Where(_ => !isCollapsed).Subscribe(_ => ModifySpawnerCoolTime(_.Value, 0.5f)).AddTo(source.Token);
        SpawnSlimes().Forget();
    }

    private void ActiveSkill()
    {
        foreach (var monster in GameManager.Instance.monsterList)
            ModifySlimeSplitCount(monster, 1);

        foreach (var spawner in GameManager.Instance.monsterSpawner)
            ModifySpawnerCoolTime(spawner, 0.5f);
    }

    private void DeactiveSkill()
    {
        foreach (var monster in GameManager.Instance.monsterList)
            ModifySlimeSplitCount(monster, -1);

        foreach (var spawner in GameManager.Instance.monsterSpawner)
            ModifySpawnerCoolTime(spawner, 2);
    }

    public override void Patrol()
    {
        //������ ������������
    }

    private void OnDestroy()
    {
        source?.Cancel();
        source?.Dispose();
    }
}
