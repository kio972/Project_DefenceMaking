using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GolemIron : Golem
{
    [SerializeField]
    protected Transform middlePos;
    [SerializeField]
    GameObject sheildPrefab;

    protected ParticleSystem _sheildEffect;

    public override void GetDamage(int damage, Battler attacker)
    {
        base.GetDamage(damage, attacker);
        if(shield <= 0)
        {
            if(_sheildEffect != null)
                _sheildEffect.gameObject.SetActive(false);
        }
    }

    public override void Init()
    {
        base.Init();
        shield += Mathf.RoundToInt(maxHp * 0.2f);
        if(_sheildEffect == null)
            _sheildEffect = Instantiate(sheildPrefab, middlePos).GetComponentInChildren<ParticleSystem>();
        if(_sheildEffect != null)
        {
            _sheildEffect.gameObject.SetActive(true);
            _sheildEffect.Play();
        }
    }
}
