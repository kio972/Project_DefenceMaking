using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBattleMain : Battler
{

    public override void GetDamage(int damage, Battler attacker)
    {
        curHp -= 1;
        if (curHp <= 0)
            Dead();

        attacker.GetDamage(attacker.maxHp + attacker.armor, this);
    }

}
