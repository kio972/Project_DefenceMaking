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

    public float delayTime = 1f;
    public float lerpTime = 0.2f;

    private float battlerCurHp;

    private bool deadBar = false;

    public void HPBarEnd()
    {
        gameObject.SetActive(false);
    }

    public void Init(Battler battler)
    {
        this.battler = battler;
        battlerCurHp = battler.curHp;
        hp_Bar.fillAmount = 1f;
        deadBar = false;
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

        float curHp = battler.curHp;
        float maxHp = battler.maxHp;
        float nextAmount = curHp / maxHp;
        hp_Bar.fillAmount = nextAmount;

        if(nextAmount <= 0)
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

        if (battlerCurHp != battler.curHp)
        {
            battlerCurHp = battler.curHp;
            UpdateHp();
        }
        UpdatePosition(position);
    }
}
