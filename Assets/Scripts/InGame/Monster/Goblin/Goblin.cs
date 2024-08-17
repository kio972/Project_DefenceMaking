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

        var tiles = PathFinder.FindPath(curTile, NodeManager.Instance.endPoint);
        int count = 0;


        TileNode targetTile = null;
        tiles.Remove(curTile);
        tiles.Remove(NodeManager.Instance.endPoint);
        float curToEndDistance = UtilHelper.CalCulateDistance(transform, NodeManager.Instance.endPoint.transform);
        if(curToEndDistance < tiles.Count)
            tiles.RemoveAt(0);

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
