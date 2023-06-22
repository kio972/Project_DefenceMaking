using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap : MonoBehaviour
{
    public int trapIndex;
    public int damage;
    public int duration;
    public float attackSpeed;
    private int attackCount = 0;

    private Dictionary<Battler, Coroutine> coroutineDic;

    private void DestroyTrap()
    {
        foreach(Coroutine co in coroutineDic.Values)
        {
            StopCoroutine(co);
        }

        Destroy(this.gameObject);
    }

    private void ExcuteAttack(Battler target)
    {
        target.GetDamage(damage);
        attackCount++;

        if (attackCount >= duration)
            DestroyTrap();
    }

    private IEnumerator IExcuteDamage(Battler battle)
    {
        while (true)
        {
            ExcuteAttack(battle);

            float attackElapsed = 0f;
            while(attackElapsed < duration)
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

    private void OnTriggerExit(Collider other)
    {
        Battler battle = other.GetComponent<Battler>();
        if (battle == null || battle.unitType == UnitType.Player) return;

        if(coroutineDic.ContainsKey(battle))
        {
            StopCoroutine(coroutineDic[battle]);
            coroutineDic.Remove(battle);
        }
    }
}
