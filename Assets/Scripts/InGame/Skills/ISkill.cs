using System.Collections;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

public interface ISkill
{
    public bool IsPassive { get; }
    public ReactiveProperty<float> coolRate { get; }
    public ReactiveProperty<bool> isReady { get; }

    public bool UseSkill();

    public void SkillInit();

    public SkillData SaveSkill();

    public void LoadSkill(SkillData data);
}

public interface IHaveCost
{
    public int cost { get; }
    public string costType { get; }
}
