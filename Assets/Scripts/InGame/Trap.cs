using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Trap : MonoBehaviour
{
    public int trapIndex;
    public int damage;
    public int duration;
    public float attackSpeed;
    private int attackCount = 0;

    private Dictionary<Battler, Coroutine> coroutineDic = new Dictionary<Battler, Coroutine>();

    private Tile curTile;

    private void DestroyTrap()
    {
        foreach(Coroutine co in coroutineDic.Values)
        {
            StopCoroutine(co);
        }

        curTile.trap = null;
        Destroy(this.gameObject);
    }

    private void ExcuteAttack(Battler target)
    {
        if(target.isDead)
        {
            RemoveTarget(target);
            return;
        }

        target.GetDamage(damage, null);
        attackCount++;

        AudioManager.Instance.Play2DSound("Attack_trap", SettingManager.Instance.fxVolume);

        if (attackCount >= duration)
            DestroyTrap();
    }

    private IEnumerator IExcuteDamage(Battler battle)
    {
        while (true)
        {
            ExcuteAttack(battle);

            float attackElapsed = 0f;
            while(attackElapsed < 1 / attackSpeed)
            {
                attackElapsed += Time.deltaTime * GameManager.Instance.timeScale;
                yield return null;
            }

            yield return null;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Battler battle = other.GetComponent<Battler>();
        if (battle == null || battle.unitType == UnitType.Player) return;

        coroutineDic.Add(battle, StartCoroutine(IExcuteDamage(battle)));
    }

    private void RemoveTarget(Battler battle)
    {
        if (coroutineDic.ContainsKey(battle))
        {
            StopCoroutine(coroutineDic[battle]);
            coroutineDic.Remove(battle);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Battler battle = other.GetComponent<Battler>();
        if (battle == null || battle.unitType == UnitType.Player) return;

        RemoveTarget(battle);
    }

    public void Init(Tile curTile)
    {
        damage = Convert.ToInt32(DataManager.Instance.Trap_Table[trapIndex]["attackPower"]);
        attackSpeed = Convert.ToInt32(DataManager.Instance.Trap_Table[trapIndex]["attackSpeed"]);
        duration = Convert.ToInt32(DataManager.Instance.Trap_Table[trapIndex]["duration"]);

        this.curTile = curTile;
        curTile.trap = this;

        Collider col = GetComponentInChildren<Collider>();
        if (col != null)
            col.enabled = true;
    }

}
