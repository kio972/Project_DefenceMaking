using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AkAnimationListener : MonoBehaviour
{
    private Battler battler;

    [SerializeField]
    private AK.Wwise.Event attackSound;

    [Space]
    [SerializeField]
    private AK.Wwise.Event deadSound;

    void Dead()
    {
        deadSound?.Post(gameObject);
    }

    void Attack()
    {
        battler.Attack();
        attackSound?.Post(gameObject);
    }

    private void Awake()
    {
        if (battler == null)
            battler = GetComponentInParent<Battler>();
    }
}
