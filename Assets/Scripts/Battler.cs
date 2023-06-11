using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Battler : MonoBehaviour
{
    public int damage;
    public int curHp;
    public int maxHp;
    public int armor;
    public float attackSpeed;

    public UnitType unitType = UnitType.Enemy;

    public virtual void Dead()
    {

    }

    public void GetDamage(int damage)
    {
        int finalDamage = damage - armor;
        if (finalDamage < 0)
            finalDamage = 1;

        curHp -= finalDamage;
        if (curHp <= 0)
            Dead();
    }


    public virtual void Init()
    {
        //string resourcePath = "";
        //if (unitType == UnitType.Enemy)
        //    resourcePath = "Prefab/UI/Adventure_hp_bar";
        //else if (unitType == UnitType.Player)
        //    resourcePath = "Prefab/UI/Monster_hp_bar";


        //HpBar hpBar = Resources.Load<HpBar>(resourcePath);
        //hpBar = Instantiate(hpBar, GameManager.Instance.cameraCanvas.transform);
        //hpBar.Init(this);
    }
}
