using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Trap : MonoBehaviour
{
    private int trapIndex = -1;
    [SerializeField]
    private string battlerID;
    public string BattlerID { get => battlerID; }

    protected int minDamage;
    protected int maxDamage;
    public int Damage { get { return UnityEngine.Random.Range(minDamage, maxDamage + 1); } }

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
        gameObject.SetActive(false);
        if (NodeManager.Instance._GuideState == GuideState.Trap)
            NodeManager.Instance.SetGuideState(GuideState.Trap);

        GameManager.Instance.trapList.Remove(this);
    }

    private void ExcuteAttack()
    {
        List<Battler> removeTargets = new List<Battler>();
        int targetCount = 0;
        foreach(Battler target in targetList)
        {
            if (targetCount >= maxTarget)
                break;

            target.GetDamage(Damage, null);
            if (target.isDead)
                removeTargets.Add(target);
            targetCount++;
        }

        attackCount++;
        AudioManager.Instance.Play2DSound("Attack_trap", SettingManager.Instance._FxVolume);

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

        minDamage = Convert.ToInt32(DataManager.Instance.Battler_Table[trapIndex]["attackPowerMin"]);
        maxDamage = Convert.ToInt32(DataManager.Instance.Battler_Table[trapIndex]["attackPowerMax"]);
        attackSpeed = Convert.ToInt32(DataManager.Instance.Battler_Table[trapIndex]["attackSpeed"]);
        duration = Convert.ToInt32(DataManager.Instance.Battler_Table[trapIndex]["duration"]);
        maxTarget = Convert.ToInt32(DataManager.Instance.Battler_Table[trapIndex]["targetCount"]);

        this.curTile = curTile;
        curTile.trap = this;
        transform.position = curTile.transform.position;

        Collider col = GetComponentInChildren<Collider>();
        if (col != null)
            col.enabled = true;

        animator = GetComponent<Animator>();

        attackCount = 0;

        GameManager.Instance.trapList.Add(this);
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
