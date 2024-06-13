using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HPBarPooling : IngameSingleton<HPBarPooling>
{
    List<HpBar> allyHpbars = new List<HpBar>();
    List<HpBar> enemyHpbar = new List<HpBar>();

    public HpBar GetHpBar(UnitType unitType, Battler battler)
    {
        bool isAlly = false;
        string resourcePath = "";
        List<HpBar> targetHpbars = enemyHpbar;
        if (unitType == UnitType.Enemy)
            resourcePath = "Prefab/UI/hp_bar_Adventure";
        else if (unitType == UnitType.Player)
        {
            resourcePath = battler is PlayerBattleMain ? "Prefab/UI/hp_bar_King" : "Prefab/UI/hp_bar_Monster";
            isAlly = true;
            targetHpbars = allyHpbars;
        }

        HpBar hpbar = null;
        foreach(HpBar target in targetHpbars)
        {
            if(!target.gameObject.activeSelf)
            {
                hpbar = target;
                hpbar.gameObject.SetActive(true);
                break;
            }
        }

        if(hpbar == null)
        {
            HpBar hpPrefab = Resources.Load<HpBar>(resourcePath);
            hpbar = Instantiate(hpPrefab, GameManager.Instance.cameraCanvas.transform);
            if (isAlly)
                allyHpbars.Add(hpbar);
            else
                enemyHpbar.Add(hpbar);
        }

        hpbar.Init(battler);
        return hpbar;
    }
}
