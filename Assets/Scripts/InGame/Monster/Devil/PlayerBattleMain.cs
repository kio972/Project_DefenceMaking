using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class PlayerBattleMain : Battler
{
    public ReactiveCollection<ISkill> skills = new ReactiveCollection<ISkill>();

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

    public void SetTile(TileNode node)
    {
        _curNode = node;
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

}
