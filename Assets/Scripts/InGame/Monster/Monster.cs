using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Monster : Battler
{
    private int monsterIndex = -1;

    [SerializeField]
    private bool isHide = false;

    public override void Dead()
    {
        base.Dead();
        GameManager.Instance.monsterList.Remove(this);
        if (curTile.curTile.monster != null && curTile.curTile.monster == this)
            curTile.curTile.monster = null;
    }

    private void ResetPaths()
    {
        crossedNodes = new List<TileNode>();
        lastCrossRoad = null;
        prevTile = null;
        directPass = false;
    }

    protected override void DirectPass(TileNode targetTile)
    {
        if (targetTile == null)
        {
            ResetPaths();
            return;
        }

        if (nextTile == null || nextTile.curTile == null)
        {
            List<TileNode> path = PathFinder.Instance.FindPath(curTile, targetTile);
            if (path != null && path.Count > 0)
                nextTile = path[0];
            else
            {
                ResetPaths();
                return;
            }
        }

        ExcuteMove(nextTile);

        // nextNode까지 이동완료
        if (Vector3.Distance(transform.position, nextTile.transform.position) < 0.001f)
            NodeAction(nextTile);
    }

    public override void Patrol()
    {
        if (directPass)
            DirectPass(lastCrossRoad);
        else
            NormalPatrol();
    }

    protected override TileNode FindNextNode(TileNode curNode)
    {
        //startNode에서 roomDirection이나 pathDirection이 있는 방향의 이웃노드를 받아옴
        List<TileNode> nextNodes = UtilHelper.GetConnectedNodes(curNode);
        //해당 노드에서 전에 갔던 노드는 제외
        nextNodes.Remove(prevTile);
        nextNodes.Remove(NodeManager.Instance.startPoint);
        nextNodes.Remove(NodeManager.Instance.endPoint);
        if (crossedNodes != null)
        {
            foreach (TileNode node in crossedNodes)
                nextNodes.Remove(node);
        }

        if (nextNodes.Count == 0)
            return null;

        //갈림길일경우 저장
        if (nextNodes.Count > 1)
            lastCrossRoad = curNode;

        TileNode nextNode = nextNodes[UnityEngine.Random.Range(0, nextNodes.Count)];
        return nextNode;
    }

    public void SetStartPoint(TileNode tile)
    {
        curTile = tile;
    }

    public override void Init()
    {
        base.Init();

        monsterIndex = UtilHelper.Find_Data_Index(battlerID, DataManager.Instance.Battler_Table, "id");
        if(monsterIndex != -1)
        {
            InitStats(monsterIndex);

            maxHp += PassiveManager.Instance.monsterHp_Weight;
            curHp = maxHp;
        }

        GameManager.Instance.monsterList.Add(this);

        if(isHide)
            InitState(this, FSMHide.Instance);
        else
            InitState(this, FSMPatrol.Instance);
    }


    public override void Update()
    {
        base.Update();
    }
}
