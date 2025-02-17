using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;

public class PlayerBattleMain : Battler
{
    public ReactiveCollection<ISkill> skills { get; private set; } = new ReactiveCollection<ISkill>();

    public bool HaveSkill<T>() where T : ISkill
    {
        foreach (var item in skills)
        {
            if (item is T)
                return true;
        }
        return false;
    }

    public bool HaveSkill<T>(out T skill) where T : ISkill
    {
        skill = default;
        foreach (var item in skills)
        {
            if (item is T targetSkill)
            {
                skill = targetSkill;
                return true;
            }
        }

        return false;
    }

    public ISkill AddSkill(ISkill skill)
    {
        // 동일한 타입의 스킬이 이미 있는지 확인
        var existingSkill = skills.FirstOrDefault(existingSkill => existingSkill.GetType() == skill.GetType());

        // 이미 존재하는 경우, 기존 스킬을 반환
        if (existingSkill != null)
            return existingSkill;

        // 새 스킬 추가
        skills.Add(skill);
        skill.SkillInit();
        return skill; // 새로 추가한 스킬 반환
    }

    public ISkill AddSkill(string skillName)
    {
        var skillType = Type.GetType(skillName);
        if (skillType != null && typeof(ISkill).IsAssignableFrom(skillType))
        {
            var skill = Activator.CreateInstance(skillType) as ISkill;
            if (skill != null)
                return AddSkill(skill);
        }

        return null;
    }

    public void SetTile(TileNode node)
    {
        _curNode = node;
    }

    public void ForceGetDamage(int damage)
    {
        curHp -= damage;
        if (curHp <= 0)
            Dead(null);

        PlayDamageText(damage, UnitType.Player, false);
        GameManager.Instance._InGameUI.StartBloodEffect();
        GameManager.Instance.cameraController.ShakeCamera();
    }

    public override void GetDamage(int damage, Battler attacker)
    {
        curHp -= 1;
        if (curHp <= 0)
            Dead(attacker);

        if (animator.GetCurrentAnimatorStateInfo(0).IsTag("IDLE") && !animator.IsInTransition(0))
            animator.SetTrigger("Damaged");

        PlayDamageText(1, UnitType.Player, false);

        //attacker.GetDamage(attacker.maxHp + attacker.armor, this);

        GameManager.Instance._InGameUI.StartBloodEffect();
        GameManager.Instance.cameraController.ShakeCamera();
    }

    public void MoveToBossRoom()
    {
        transform.position = NodeManager.Instance.endPoint.transform.position;
        Tile endTile = NodeManager.Instance.endPoint.curTile;
        Direction endTileExitDirection = endTile.PathDirection[0];
        Vector3 exitNode = NodeManager.Instance.endPoint.neighborNodeDic[endTileExitDirection].transform.position;
        RotateCharacter(exitNode);
    }
    public override void Play_AttackAnimation()
    {
        base.Play_AttackAnimation();
        if (curTarget != null)
            EffectPooling.Instance.PlayEffect("DarkBall", transform, new Vector3(0, 0.5f, 0), 0.4f, curTarget.transform.position);
    }

    public override void Update()
    {
        base.Update();

        if ((transform.position - NodeManager.Instance.endPoint.transform.position).magnitude > 0.01f)
            MoveToBossRoom();
    }

    public override void Init()
    {
        base.Init();

        int index = UtilHelper.Find_Data_Index(battlerID, DataManager.Instance.battler_Table, "id");
        if(index != -1)
        {
            InitStats(index);
        }

        _curNode = NodeManager.Instance.endPoint;
        MoveToBossRoom();

        InitState(this, FSMKing.Instance);
    }

    public override BattlerData GetData()
    {
        BattlerData target = base.GetData();
        target.skills = new List<SkillData>();
        foreach(var skill in skills)
        {
            target.skills.Add(skill.SaveSkill());
        }

        return target;
    }

    public override void LoadData(BattlerData data)
    {
        curHp = data.curHp;
        foreach (var skillData in data.skills)
        {
            ISkill skill = AddSkill(skillData.skillName);
            skill?.LoadSkill(skillData);
        }
    }
}
