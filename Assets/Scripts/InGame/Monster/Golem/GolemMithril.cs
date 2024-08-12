using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;

public class GolemMithril : GolemIron
{
    private CancellationTokenSource tokenSource = new CancellationTokenSource();

    public override void Dead()
    {
        tokenSource.Cancel();
        base.Dead();
    }

    private async UniTaskVoid ShieldRestoration(float time)
    {
        float elapsedTime = 0f;
        while(elapsedTime < time)
        {
            elapsedTime += GameManager.Instance.InGameDeltaTime;
            await UniTask.Yield(tokenSource.Token);
        }

        if (curHp >= 10 && shield <= 0)
            shield = Mathf.RoundToInt(maxHp * 0.2f);
    }

    private void ShieldExplosion()
    {
        int explosionDamage = Mathf.FloorToInt(maxHp * 0.2f * 0.5f);
        List<Battler> targets = GetRangedTargets(transform.position, 1f, false);
        foreach (var target in targets)
            target.GetDamage(explosionDamage, this);

        ShieldRestoration(12 * 60).Forget();
    }

    public override void GetDamage(int damage, Battler attacker)
    {
        bool haveSheild = shield > 0;
        base.GetDamage(damage, attacker);
        if (haveSheild && shield <= 0)
            ShieldExplosion();
    }

    public override void Init()
    {
        base.Init();
        tokenSource = new CancellationTokenSource();
    }
}
