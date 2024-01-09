using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Monster : Battler
{
    private int monsterIndex = -1;

    [SerializeField]
    private bool isHide = false;
    [SerializeField]
    private int holdBackCount = 1;

    public bool CanHoldBack
    {
        get
        {
            bool isListHaveEmpty = rangedTargets.Count < holdBackCount;
            return isListHaveEmpty;
        }
    }


    public override void Dead()
    {
        base.Dead();
        GameManager.Instance.monsterList.Remove(this);
        //if (curTile.curTile.monster != null && curTile.curTile.monster == this)
        //{
        //    curTile.curTile.monster = null;
        //    if (NodeManager.Instance._GuideState == GuideState.Monster)
        //        NodeManager.Instance.SetGuideState(GuideState.Monster);
        //}
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

        // nextNode���� �̵��Ϸ�
        if (Vector3.Distance(transform.position, nextTile.transform.position) < 0.001f)
            NodeAction(nextTile);
    }

    public override void Patrol()
    {
        if(PathFinder.Instance.FindPath(curTile, NodeManager.Instance.endPoint) == null)
        {
            collapseCool += Time.deltaTime * GameManager.Instance.timeScale;
            if(collapseCool >= 1f)
            {
                collapseCool = 0f;
                GetDamage(1, null);
            }
        }

        if (directPass)
            DirectPass(lastCrossRoad);
        else
            NormalPatrol();
    }

    protected override TileNode FindNextNode(TileNode curNode)
    {
        //startNode���� roomDirection�̳� pathDirection�� �ִ� ������ �̿���带 �޾ƿ�
        List<TileNode> nextNodes = UtilHelper.GetConnectedNodes(curNode);
        //�ش� ��忡�� ���� ���� ���� ����
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

        //�������ϰ�� ����
        if (nextNodes.Count > 1)
            lastCrossRoad = curNode;

        TileNode nextNode = nextNodes[UnityEngine.Random.Range(0, nextNodes.Count)];
        return nextNode;
    }

    public void SetStartPoint(TileNode tile)
    {
        transform.position = tile.transform.position;
        curTile = tile;
    }

    public override void Init()
    {
        base.Init();

        ResetNode();

        monsterIndex = UtilHelper.Find_Data_Index(battlerID, DataManager.Instance.Battler_Table, "id");
        if(monsterIndex != -1)
        {
            InitStats(monsterIndex);

            armor += PassiveManager.Instance.monsterDefense_Weight;
            attackSpeed *= ((100 + PassiveManager.Instance.monsterAttackSpeed_Weight) / 100);
            minDamage = (int)((float)minDamage * ((100 + PassiveManager.Instance.monsterDamageRate_Weight) / 100));
            maxDamage = (int)((float)maxDamage * ((100 + PassiveManager.Instance.monsterDamageRate_Weight) / 100));

            maxHp = (int)((float)maxHp * ((100 + PassiveManager.Instance.monsterHpRate_Weight) / 100));
            maxHp += PassiveManager.Instance.monsterHp_Weight;
            curHp = maxHp;

            holdBackCount = Convert.ToInt32(DataManager.Instance.Battler_Table[monsterIndex]["holdbackCount"]);
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
