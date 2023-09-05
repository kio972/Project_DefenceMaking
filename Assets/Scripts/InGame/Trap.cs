using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Trap : MonoBehaviour
{
    private int trapIndex = -1;
    [SerializeField]
    private string battlerID;

    private int damage;
    private int duration;
    private float attackSpeed;
    private int attackCount = 0;
    private int maxTarget = 1;

    private Dictionary<Battler, Coroutine> coroutineDic = new Dictionary<Battler, Coroutine>();

    private List<Battler> targetList = new List<Battler>();

    private Tile curTile;

    private Animator animator;

    private float coolDown = 0f;

    private bool isInit = false;

    private void DestroyTrap()
    {
        curTile.trap = null;
        Destroy(this.gameObject);
    }

    private void ExcuteAttack()
    {
        List<Battler> removeTargets = new List<Battler>();
        int targetCount = 0;
        foreach(Battler target in targetList)
        {
            if (targetCount >= maxTarget)
                break;

            target.GetDamage(damage, null);
            if (target.isDead)
                removeTargets.Add(target);
            targetCount++;
        }

        attackCount++;
        AudioManager.Instance.Play2DSound("Attack_trap", SettingManager.Instance.fxVolume);

        foreach (Battler removeTarget in removeTargets)
            targetList.Remove(removeTarget);
    }

    private void OnTriggerEnter(Collider other)
    {
        Battler battle = other.GetComponent<Battler>();
        if (battle == null || battle.unitType == UnitType.Player) return;

        targetList.Add(battle);
    }

    private void OnTriggerExit(Collider other)
    {
        Battler battle = other.GetComponent<Battler>();
        if (battle == null || battle.unitType == UnitType.Player) return;

        targetList.Remove(battle);
    }

    public void Init(Tile curTile)
    {
        trapIndex = UtilHelper.Find_Data_Index(battlerID, DataManager.Instance.Battler_Table, "id");

        damage = Convert.ToInt32(DataManager.Instance.Battler_Table[trapIndex]["attackPower"]);
        attackSpeed = Convert.ToInt32(DataManager.Instance.Battler_Table[trapIndex]["attackSpeed"]);
        duration = Convert.ToInt32(DataManager.Instance.Battler_Table[trapIndex]["duration"]);
        maxTarget = Convert.ToInt32(DataManager.Instance.Battler_Table[trapIndex]["targetCount"]);

        this.curTile = curTile;
        curTile.trap = this;

        Collider col = GetComponentInChildren<Collider>();
        if (col != null)
            col.enabled = true;

        animator = GetComponent<Animator>();

        isInit = true;
    }

    private void Update()
    {
        if (!isInit)
            return;

        if (attackCount >= duration)
            DestroyTrap();

        if(coolDown > 0)
            coolDown -= Time.deltaTime * GameManager.Instance.timeScale;

        if (targetList.Count == 0)
        {
            if (animator != null)
                animator.ResetTrigger("Attack");
            return;
        }

        if (GameManager.Instance.timeScale == 0) return;

        if (coolDown <= 0)
        {
            if (animator != null)
                animator.SetTrigger("Attack");

            ExcuteAttack();
            coolDown =  1 / attackSpeed;
        }
    }
}
