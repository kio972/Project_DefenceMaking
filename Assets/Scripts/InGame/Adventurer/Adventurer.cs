using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Adventurer : Battler
{
    public int adventurerIndex;
    private bool directPass = false;
    private Coroutine directPassCoroutine = null;

    private void OnTriggerEnter(Collider other)
    {
        if (battleState) return;

        Battler battle = other.GetComponent<Battler>();
        if (battle == null || battle.unitType == UnitType.Enemy) return;

        if(!battle.isDead)
        {
            battleState = true;
            curTarget = battle;
            curTarget.battleState = true;
        }
    }

    //private void ArriveEndPoint()
    //{
    //    GameManager.Instance.adventurersList.Remove(this);
    //    StopCoroutine(moveCoroutine);
    //    animator.SetBool("Attack", true);
        
    //    PlayerBattleMain king = FindObjectOfType<PlayerBattleMain>();
    //    king.GetDamage(1);
        
    //    this.GetDamage(maxHp);
    //}

    public override void Dead()
    {
        base.Dead();

        GameManager.Instance.adventurersList.Remove(this);
        StopAllCoroutines();
        animator.SetBool("Die", true);
        isDead = true;
        Invoke("RemoveBody", 2.5f);
    }

    private void RemoveBody()
    {
        gameObject.SetActive(false);
    }

    public void EndPointMoved()
    {
        if (!directPass)
            return;

        if(directPassCoroutine != null)
            StopCoroutine(directPassCoroutine);
        directPassCoroutine = StartCoroutine(DirectPass());
    }

    private IEnumerator DirectPass()
    {
        List<TileNode> path = PathFinder.Instance.FindPath(curTile);
        Vector3 finalPos = NodeManager.Instance.endPoint.transform.position;
        foreach (TileNode node in path)
        {
            if (moveCoroutine != null)
                StopCoroutine(moveCoroutine);
            yield return moveCoroutine = StartCoroutine(Move(node.transform.position,
            () => { NodeAction(node); }));
        }
    }

    protected override void NodeAction(TileNode nextNode)
    {
        prevTile = curTile;
        curTile = nextNode;
        if (!crossedNodes.Contains(curTile))
            crossedNodes.Add(curTile);

        if(!afterCrossPath.Contains(curTile))
            afterCrossPath.Add(curTile);
    }

    protected override void DeadLock_Logic_Move()
    {
        directPass = true;
        directPassCoroutine = StartCoroutine(DirectPass());
    }



    public override void Init()
    {
        base.Init();
        StartCoroutine(MoveLogic());
    }

    public override void Update()
    {
        base.Update();

        if (battleState)
            ExcuteBattle();
    }
}
