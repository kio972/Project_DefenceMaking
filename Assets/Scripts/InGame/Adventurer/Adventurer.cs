using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class Adventurer : Battler
{
    private int adventurerIndex = -1;

    private int reward;

    protected override void RemoveBody()
    {
        base.RemoveBody();
        GameManager.Instance.adventurersList.Remove(this);
    }

    public override void Dead()
    {
        base.Dead();
        GameManager.Instance.gold += reward;
    }

    public override void Init()
    {
        base.Init();

        transform.position = NodeManager.Instance.startPoint.transform.position;
        curTile = NodeManager.Instance.startPoint;

        adventurerIndex = UtilHelper.Find_Data_Index(battlerID, DataManager.Instance.Battler_Table, "id");
        if(adventurerIndex != -1)
        {
            InitStats(adventurerIndex);

            attackSpeed *= ((100 + PassiveManager.Instance.adventurerAttackSpeed_Weight) / 100);
            minDamage = (int)((float)minDamage * ((100 + PassiveManager.Instance.adventurerDamageRate_Weight) / 100));
            maxDamage = (int)((float)maxDamage * ((100 + PassiveManager.Instance.adventurerDamageRate_Weight) / 100));

            reward = Convert.ToInt32(DataManager.Instance.Battler_Table[adventurerIndex]["reward"]);
        }

        InitState(this, FSMPatrol.Instance);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = UnityEngine.Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }

    public override void Update()
    {
        base.Update();

        if (animator != null)
            animator.SetFloat("AttackSpeed", attackSpeed * GameManager.Instance.timeScale);
    }
}
