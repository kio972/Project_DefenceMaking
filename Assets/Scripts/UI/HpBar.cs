using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HpBar : MonoBehaviour
{
    [SerializeField]
    private Battler battler;
    [SerializeField]
    private Image hp_Bar;
    [SerializeField]
    private GameObject imgGroup;
    [SerializeField]
    private Image shield_Bar;

    public float delayTime = 1f;
    public float lerpTime = 0.2f;

    private float battlerCurHp;
    private float battlerCurSheild;

    private bool deadBar = false;

    public void HPBarEnd()
    {
        StatusEffectUI[] effectUIs = GetComponentsInChildren<StatusEffectUI>(true);
        foreach (var item in effectUIs)
            item.DisableUI();

        gameObject.SetActive(false);
    }

    public void Init(Battler battler)
    {
        this.battler = battler;
        battlerCurHp = battler.curHp;
        hp_Bar.fillAmount = 1f;
        deadBar = false;
        shield_Bar.gameObject.SetActive(battler.shield != 0);
        
        if(battler is not PlayerBattleMain)
            imgGroup.SetActive(false);

        StatusEffectUI[] effectUIs = GetComponentsInChildren<StatusEffectUI>(true);
        foreach (var item in effectUIs)
            item.Init(battler);
    }


    private void UpdatePosition(Vector3 position)
    {
        if (battler == null)
            return;

        RectTransform rect = transform.GetComponent<RectTransform>();
        rect.position = position;
    }

    public void UpdateHp()
    {
        if(deadBar) return;

        transform.SetAsLastSibling();
        bool haveShield = battler.shield != 0;
        shield_Bar.gameObject.SetActive(haveShield);

        float curHp = battler.curHp;
        float maxHp = battler.maxHp;
        float sheldHp = curHp + battler.shield;
        if (haveShield)
        {
            maxHp = Mathf.Max(sheldHp, maxHp);
            shield_Bar.fillAmount = sheldHp / maxHp;
        }

        hp_Bar.fillAmount = curHp / maxHp;

        if(curHp <= 0)
        {
            deadBar = true;
            Invoke("HPBarEnd", 0.2f);
        }
    }

    // Update is called once per frame
    public void UpdateHpBar(Vector3 position)
    {
        if (battler == null)
            return;

        if(imgGroup != null)
            imgGroup.SetActive(hp_Bar.fillAmount < 1 || battler is PlayerBattleMain);

        if (battlerCurHp != battler.curHp || battlerCurSheild != battler.shield)
        {
            battlerCurHp = battler.curHp;
            battlerCurSheild = battler.shield;
            UpdateHp();
        }
        UpdatePosition(position);
    }
}
