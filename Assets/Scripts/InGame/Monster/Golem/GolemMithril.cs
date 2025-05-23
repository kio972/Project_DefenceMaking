using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;

public class GolemMithril : GolemObsidian
{
    private CancellationTokenSource tokenSource = new CancellationTokenSource();

    public override void Dead(Battler attacker)
    {
        tokenSource.Cancel();
        base.Dead(attacker);
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

        if (_sheildEffect != null)
        {
            _sheildEffect.gameObject.SetActive(true);
            _sheildEffect.Play();
        }
    }

    protected override void ShieldExplosion()
    {
        base.ShieldExplosion();
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
