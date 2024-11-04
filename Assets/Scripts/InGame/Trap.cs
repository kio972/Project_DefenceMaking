using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UniRx;

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
    public int Duration { get { return curDuration.Value; } }
    private float attackSpeed;
    private int attackCount = 0;
    private int maxTarget = 1;

    private Dictionary<Battler, Coroutine> coroutineDic = new Dictionary<Battler, Coroutine>();

    private List<Battler> targetList = new List<Battler>();

    private Tile curTile;

    private Animator animator;

    private float coolDown = 0f;

    private bool isInit = false;

    public ReactiveProperty<int> curDuration = new ReactiveProperty<int>();
    public ReactiveProperty<int> maxDuration = new ReactiveProperty<int>();

    [SerializeField]
    AudioClip attackSound;

    public void DestroyTrap()
    {
        curTile.trap = null;
        gameObject.SetActive(false);
        if (NodeManager.Instance._GuideState == GuideState.Trap)
            NodeManager.Instance.SetGuideState(GuideState.Trap);

        transform.position = Vector3.up * 1000f;
        GameManager.Instance.trapList.Remove(this);
        isInit = false;
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
        curDuration.Value = duration - attackCount;
        AudioManager.Instance.Play3DSound(attackSound, transform.position, SettingManager.Instance._FxVolume);

        foreach (Battler removeTarget in removeTargets)
            targetList.Remove(removeTarget);
    }

    private void OnTriggerEnter(Collider other)
    {
        Battler battle = other.GetComponent<Battler>();
        if (battle == null || battle.unitType == UnitType.Player) return;

        if(battle.HaveEffect<Stealth_sup>())
            return;

        targetList.Add(battle);
    }

    private void OnTriggerExit(Collider other)
    {
        Battler battle = other.GetComponent<Battler>();
        if (battle == null || battle.unitType == UnitType.Player) return;

        targetList.Remove(battle);
    }

    public void Init(Tile curTile, int startDuration = 0)
    {
        trapIndex = UtilHelper.Find_Data_Index(battlerID, DataManager.Instance.battler_Table, "id");

        minDamage = Convert.ToInt32(DataManager.Instance.battler_Table[trapIndex]["attackPowerMin"]);
        maxDamage = Convert.ToInt32(DataManager.Instance.battler_Table[trapIndex]["attackPowerMax"]);
        attackSpeed = Convert.ToInt32(DataManager.Instance.battler_Table[trapIndex]["attackSpeed"]);
        duration = Convert.ToInt32(DataManager.Instance.battler_Table[trapIndex]["duration"]);
        maxTarget = Convert.ToInt32(DataManager.Instance.battler_Table[trapIndex]["targetCount"]);

        this.curTile = curTile;
        curTile.trap = this;
        transform.position = curTile.transform.position;

        Collider col = GetComponentInChildren<Collider>();
        if (col != null)
            col.enabled = true;

        animator = GetComponentInChildren<Animator>();

        attackCount = 0;
        coolDown = 0.1f;

        GameManager.Instance.trapList.Add(this);
        targetList = new List<Battler>();

        maxDuration.Value = duration;
        curDuration.Value = startDuration == 0 ? duration : startDuration;
        HPBarPooling.Instance.GetTrapHpBar(this);
        isInit = true;
    }

    private void DeadTargetCheck()
    {
        List<Battler> removeTargets = new List<Battler>();
        foreach (Battler target in targetList)
        {
            if (target.isDead)
                removeTargets.Add(target);
        }

        foreach (Battler removeTarget in removeTargets)
            targetList.Remove(removeTarget);
    }

    private void Update()
    {
        if (!isInit)
            return;

        if(coolDown > 0)
            coolDown -= Time.deltaTime * GameManager.Instance.timeScale;

        DeadTargetCheck();
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

        if (attackCount >= duration)
            DestroyTrap();
    }
}
