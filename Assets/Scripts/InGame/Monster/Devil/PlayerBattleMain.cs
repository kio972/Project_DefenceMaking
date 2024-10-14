using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class PlayerBattleMain : Battler
{
    public ReactiveCollection<ISkill> skills = new ReactiveCollection<ISkill>();

    public void SetTile(TileNode node)
    {
        curTile = node;
    }

    public override void GetDamage(int damage, Battler attacker)
    {
        curHp -= 1;
        if (curHp <= 0)
            Dead();

        if (animator.GetCurrentAnimatorStateInfo(0).IsTag("IDLE") && !animator.IsInTransition(0))
            animator.SetTrigger("Damaged");

        PlayDamageText(1, UnitType.Player, false);

        //attacker.GetDamage(attacker.maxHp + attacker.armor, this);

        GameManager.Instance._InGameUI.StartBloodEffect();
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

        int index = UtilHelper.Find_Data_Index(battlerID, DataManager.Instance.Battler_Table, "id");
        if(index != -1)
        {
            InitStats(index);
        }

        curTile = NodeManager.Instance.endPoint;
        MoveToBossRoom();

        InitState(this, FSMKing.Instance);
    }

}
