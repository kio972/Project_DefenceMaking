using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slime : Monster
{
    private int splitCount = 3;
    private TileNode splitedNode = null;
    private TileNode slime_curNode = null;
    private bool skipedFirst = false;

    public void OnDisable()
    {
        RotationAxis.localScale = Vector3.one;
        splitCount = 3;
        splitedNode = null;
        slime_curNode = null;
    }

    private bool IsTileChanged()
    {
        if (slime_curNode == curTile || 0.05f < Vector3.Distance(curTile.transform.position, transform.position))
            return false;
        slime_curNode = curTile;
        if(!skipedFirst)
        {
            skipedFirst = true;
            return false;
        }

        return true;
    }

    private List<TileNode> GetNextNodes(TileNode curNode, TileNode prevNode)
    {
        List<TileNode> nextNodes = UtilHelper.GetConnectedNodes(curNode);
        //해당 노드에서 전에 갔던 노드는 제외
        nextNodes.Remove(prevTile);
        nextNodes.Remove(GameManager.Instance.king.CurTile);
        return nextNodes;
    }

    private void ExcuteSplit(List<TileNode> nextNodes)
    {
        splitCount--;
        splitedNode = curTile;
        bool isOdd = curHp % 2 == 1;
        curHp /= 2;
        maxHp /= 2;
        BattlerData curData = GetData();
        RotationAxis.localScale *= 0.7f;
        TileNode randomNode = nextNodes[Random.Range(0, nextNodes.Count)];
        this.nextTile = randomNode;
        nextNodes.Remove(randomNode);

        Monster monster = BattlerPooling.Instance.SpawnMonster(curData.id, NodeManager.Instance.FindNode(curData.row, curData.col), "id");
        monster.LoadData(curData);
        Slime newSlime = monster.GetComponent<Slime>();
        newSlime.RotationAxis.localScale = RotationAxis.localScale;
        newSlime.splitCount = this.splitCount;
        newSlime.splitedNode = this.splitedNode;
        newSlime.nextTile = nextNodes[Random.Range(0, nextNodes.Count)];

        if (isOdd) { curHp++; maxHp++; }
    }

    public override void Patrol()
    {
        base.Patrol();
        if (splitCount <= 0 || curHp < 2)
            return;

        if (!IsTileChanged())
            return;

        List<TileNode> nextNodes = GetNextNodes(slime_curNode, prevTile);
        if (nextNodes.Count < 2)
            return;

        //ChangeState(FSMSkill.Instance);
        ExcuteSplit(nextNodes);
    }
}
