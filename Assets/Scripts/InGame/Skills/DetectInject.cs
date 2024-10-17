using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

public class DetectInject : ISkill
{
    public bool IsPassive { get => true; }

    private float coolTime = 4320;
    private float curCoolTime = 0;

    private Battler king;

    private int _range = 2;
    private int _duration = 720;

    public ReactiveProperty<float> coolRate { get; } = new ReactiveProperty<float>(0);

    public ReactiveProperty<bool> isReady { get; } = new ReactiveProperty<bool>(false);

    //private float _devilAuraRange;
    //private int _devilAuraPower;

    public bool UseSkill()
    {
        Monster target = null;
        float minDist = _range + 0.1f;
        foreach (Monster monster in GameManager.Instance._MonsterList)
        {
            if (monster.HaveEffect<Detect>())
                continue;

            float dist = UtilHelper.CalCulateDistance(king.transform, monster.transform);
            if (dist > _range)
                continue;

            if (dist < minDist)
            {
                target = monster;
                minDist = dist;
            }
        }

        if(target != null)
        {
            target.AddStatusEffect<Detect>(new Detect(target, _duration));
            return true;
        }
        else
            return false;
    }

    private readonly float scanDelay = 0.2f;

    private async UniTaskVoid PassiveEffect()
    {
        while (!king.isDead)
        {
            await UniTask.Delay(System.TimeSpan.FromSeconds(scanDelay), false, cancellationToken: king.GetCancellationTokenOnDestroy());

            if (curCoolTime > 0)
                continue;

            bool skillActived = UseSkill();
            if(skillActived)
                curCoolTime = coolTime;
        }
    }

    public void SkillInit()
    {
        king = GameManager.Instance.king;
        PassiveEffect().Forget();

        var coolTimeStream = Observable.EveryUpdate().Where(_ => curCoolTime > 0)
            .Subscribe(_ =>
            {
                curCoolTime -= GameManager.Instance.InGameDeltaTime;
                if (curCoolTime < 0)
                    curCoolTime = 0;

                coolRate.Value = curCoolTime <= 0 ? 0 : curCoolTime / coolTime;
            }).AddTo(king.gameObject);
    }
}
