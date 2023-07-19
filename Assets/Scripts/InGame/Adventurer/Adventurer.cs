using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Adventurer : Battler
{
    private int adventurerIndex = -1;
    private bool directPass = false;
    private Coroutine directPassCoroutine = null;

    private int reward;

    //private void OnTriggerEnter(Collider other)
    //{
    //    Battler battle = other.GetComponent<Battler>();
    //    if (battle == null || battle.unitType == UnitType.Enemy) return;

    //    if(!battle.isDead)
    //    {
    //        rangedTargets.Add(battle);
    //        battleState = true;

    //        curTarget = FindNextTarget();
    //        if (curTarget == null)
    //            return;

    //        if(!curTarget.battleState)
    //        {
    //            curTarget.battleState = true;
    //            curTarget.curTarget = this;
    //            curTarget.RotateCharacter(transform.position);
    //        }
    //    }
    //}

    //private void OnTriggerExit(Collider other)
    //{
    //    Battler battle = other.GetComponent<Battler>();
    //    if (battle == null || battle.unitType == UnitType.Enemy) return;

    //    rangedTargets.Remove(battle);
    //    if (curTarget == battle)
    //        curTarget = null;
    //}

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
        GameManager.Instance.gold += reward;
        GameManager.Instance.adventurersList.Remove(this);
        
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
        while(true)
        {
            yield return null;
            List<TileNode> path = PathFinder.Instance.FindPath(curTile);
            if (path == null)
                continue;
            Vector3 finalPos = NodeManager.Instance.endPoint.transform.position;
            foreach (TileNode node in path)
            {
                if (moveCoroutine != null)
                    StopCoroutine(moveCoroutine);
                yield return moveCoroutine = StartCoroutine(Move(node, () => { NodeAction(node); }));
                yield return null;
            }
        }
        
    }

    protected override void DeadLock_Logic_Move()
    {
        directPass = true;
        directPassCoroutine = StartCoroutine(DirectPass());
    }



    public override void Init()
    {
        base.Init();

        adventurerIndex = UtilHelper.Find_Data_Index(battlerID, DataManager.Instance.Battler_Table, "id");
        if(adventurerIndex != -1)
        {
            maxHp = Convert.ToInt32(DataManager.Instance.Battler_Table[adventurerIndex]["hp"]);
            curHp = maxHp;
            damage = Convert.ToInt32(DataManager.Instance.Battler_Table[adventurerIndex]["attackPower"]);
            float.TryParse(DataManager.Instance.Battler_Table[adventurerIndex]["attackSpeed"].ToString(), out attackSpeed);
            armor = Convert.ToInt32(DataManager.Instance.Battler_Table[adventurerIndex]["armor"]);
            float.TryParse(DataManager.Instance.Battler_Table[adventurerIndex]["moveSpeed"].ToString(), out moveSpeed);
            reward = Convert.ToInt32(DataManager.Instance.Battler_Table[adventurerIndex]["reward"]);
            float.TryParse(DataManager.Instance.Battler_Table[adventurerIndex]["attackRange"].ToString(), out attackRange);
        }

        StartCoroutine(MoveLogic());
    }

    private bool BattleCheck()
    {
        curTarget = null;
        //본인 주변 attackRange만큼 spherecastAll실행
        Collider[] colliders = new Collider[10];
        int colliderCount = Physics.OverlapSphereNonAlloc(transform.position, attackRange, colliders, LayerMask.GetMask("Character"));
        for (int i = 0; i < colliderCount; i++)
        {
            Battler battle = colliders[i].GetComponent<Battler>();
            if (battle == null || battle.unitType == UnitType.Enemy)
                continue;

            if (curTarget == null)
                curTarget = battle;
            else if (Vector3.Distance(transform.position, battle.transform.position) <
                Vector3.Distance(transform.position, curTarget.transform.position))
                curTarget = battle;
        }

        if (curTarget == null)
        {
            battleState = false;
            return false;
        }
        else
        {
            battleState = true;
            return true;
        }
    }

    public override void Update()
    {
        base.Update();

        if (BattleCheck())
            ExcuteBattle();
    }
}
