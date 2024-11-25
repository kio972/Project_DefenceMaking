using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Goblin : Monster
{
    private bool isSkillActived = false;

    private async UniTaskVoid RunAway()
    {
        curTarget = null;

        var tiles = NodeManager.Instance.FindPath(_curNode, NodeManager.Instance.endPoint);
        int count = 0;

        TileNode targetTile = null;
        tiles.Remove(_curNode);
        tiles.Remove(NodeManager.Instance.endPoint);

        if (tiles.Count == 0)
            return;

        foreach(var tile in tiles)
        {
            if (count >= 5)
                break;

            count++;
            targetTile = tile;
        }

        directPassNode = targetTile;
        animator.SetBool("RunAway", true);
        ChangeState(FSMDirectMove.Instance);
        await UniTask.WaitUntil(() => (object)CurState != FSMDirectMove.Instance, default, unitaskCancelTokenSource.Token);
        animator.SetBool("RunAway", false);
        if(PassiveManager.Instance.isGoblinHealActive)
            GetHeal(Mathf.RoundToInt(maxHp * 0.2f), this);
    }

    public override void GetDamage(int damage, Battler attacker)
    {
        base.GetDamage(damage, attacker);
        if(!isSkillActived && (float)curHp / maxHp < 0.2f)
        {
            isSkillActived = true;
            RunAway().Forget();
        }
    }

    public override void Init()
    {
        base.Init();
        isSkillActived = false;
    }
}
