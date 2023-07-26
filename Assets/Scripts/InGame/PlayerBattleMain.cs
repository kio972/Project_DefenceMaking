using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBattleMain : Battler
{

    public override void GetDamage(int damage, Battler attacker)
    {
        curHp -= 1;
        if (curHp <= 0)
            Dead();

        attacker.GetDamage(attacker.maxHp + attacker.armor, this);
    }

    public void MoveToBossRoom()
    {
        transform.position = NodeManager.Instance.endPoint.transform.position;
        Tile endTile = NodeManager.Instance.endPoint.curTile;
        Direction endTileExitDirection = endTile.PathDirection[0];
        Vector3 exitNode = NodeManager.Instance.endPoint.neighborNodeDic[endTileExitDirection].transform.position;
        RotateCharacter(exitNode);
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

        MoveToBossRoom();
    }

}
