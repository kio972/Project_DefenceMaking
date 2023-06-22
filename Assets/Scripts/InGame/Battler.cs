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
    public float moveSpeed;

    public UnitType unitType = UnitType.Enemy;

    private HpBar hpBar;

    [SerializeField]
    private Transform rotatonAxis;
    [SerializeField]
    private Transform hpPivot;

    public bool battleState = false;
    public Battler curTarget;

    public virtual void Dead()
    {
        hpBar.UpdateHp();
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

        if(this.hpBar == null)
        {
            string resourcePath = "";
            if (unitType == UnitType.Enemy)
                resourcePath = "Prefab/UI/Adventure_hp_bar";
            else if (unitType == UnitType.Player)
                resourcePath = "Prefab/UI/Monster_hp_bar";


            HpBar hpBar = Resources.Load<HpBar>(resourcePath);
            hpBar = Instantiate(hpBar, GameManager.Instance.cameraCanvas.transform);
            hpBar.Init(this);

            this.hpBar = hpBar;
        }
    }

    private void LookAtCamera()
    {
        Vector3 cameraPosition = Camera.main.transform.position;

        // 스프라이트 오브젝트의 위치와 카메라의 위치를 기준으로 방향 벡터를 계산합니다.
        Vector3 direction = cameraPosition - transform.position;

        // 방향 벡터의 각도를 구합니다.
        float angle = Mathf.Atan2(direction.z, direction.y) * Mathf.Rad2Deg;

        // 오브젝트의 rotation 값을 설정하여 카메라 방향을 바라보도록 합니다.
        rotatonAxis.transform.rotation = Quaternion.Euler(angle + 70f, 0f, 0f);
    }

    public virtual void Update()
    {
        if (rotatonAxis != null)
            LookAtCamera();

        if (hpBar != null)
            hpBar.UpdateHpBar(hpPivot.position);
    }
}
