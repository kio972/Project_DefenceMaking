using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class Adventurer : Battler
{
    private int adventurerIndex = -1;

    private int reward;


    public override void Dead()
    {
        base.Dead();
        GameManager.Instance.gold += reward;
        GameManager.Instance.adventurersList.Remove(this);
        
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
