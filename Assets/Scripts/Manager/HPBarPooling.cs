using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HPBarPooling : IngameSingleton<HPBarPooling>
{
    List<HpBar> allyHpbars = new List<HpBar>();
    List<HpBar> enemyHpbar = new List<HpBar>();

    List<TrapDurationBar> trapHpbar = new List<TrapDurationBar>();

    private T GetNext<T>(List<T> targetHpbars, string prefabPath) where T : MonoBehaviour
    {
        T hpbar = null;
        foreach (T target in targetHpbars)
        {
            if (!target.gameObject.activeSelf)
            {
                hpbar = target;
                hpbar.gameObject.SetActive(true);
                return hpbar;
            }
        }

        if (hpbar == null)
        {
            T hpPrefab = Resources.Load<T>(prefabPath);
            hpbar = Instantiate(hpPrefab, GameManager.Instance.cameraCanvas.transform);
            targetHpbars.Add(hpbar);
        }

        return hpbar;
    }

    public TrapDurationBar GetTrapHpBar(Trap trap)
    {
        TrapDurationBar hpbar = GetNext(trapHpbar, "Prefab/UI/hp_bar_Trap");
        hpbar.Init(trap);
        return hpbar;
    }

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
        if (isAlly)
            hpbar = GetNext(allyHpbars, resourcePath);
        else
            hpbar = GetNext(enemyHpbar, resourcePath);

        hpbar.Init(battler);
        return hpbar;
    }
}
