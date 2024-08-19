using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;

public class Slime : Monster
{
    private int splitCount = 2;
    private TileNode splitedNode = null;
    private TileNode slime_curNode = null;
    private bool skipedFirst = false;

    private float splitElapsed = 0;

    private CancellationTokenSource source;

    public override void Dead()
    {
        source.Cancel();
        source.Dispose();
        base.Dead();
    }

    public void UpgradeSplitCount(int value)
    {
        splitCount += value;
    }

    public override void Init()
    {
        base.Init();
        splitElapsed = 0;
        splitCount = 2 + PassiveManager.Instance._slimeSplit_Weight.Value;
        RotationAxis.localScale = Vector3.one;
        splitedNode = null;
        slime_curNode = curTile;
        source = new CancellationTokenSource();
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

    private async UniTaskVoid ExcuteSplit(List<TileNode> nextNodes)
    {
        _Animator.SetTrigger("Split");
        splitElapsed = 1.5f;
        while(splitElapsed > 0)
        {
            splitElapsed -= Time.deltaTime * GameManager.Instance.timeScale;
            await UniTask.Yield(source.Token);
        }
        
        splitCount--;
        splitedNode = curTile;
        bool isOdd = curHp % 2 == 1;
        curHp /= 2;
        maxHp /= 2;
        BattlerData curData = GetData();
        RotationAxis.localScale *= 0.8f;
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
        if(splitElapsed > 0)
            return;

        base.Patrol();
        if (splitCount <= 0 || curHp < 2)
            return;

        if (!IsTileChanged())
            return;

        List<TileNode> nextNodes = GetNextNodes(slime_curNode, prevTile);
        if (nextNodes.Count < 2)
            return;

        ExcuteSplit(nextNodes).Forget();
    }
}
