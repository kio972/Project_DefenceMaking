using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AkAnimationListener : MonoBehaviour
{
    private Battler battler;

    [SerializeField]
    FMODUnity.EventReference attackSound;

    [Space]
    [SerializeField]
    FMODUnity.EventReference deadSound;

    void Dead()
    {
        FMODUnity.RuntimeManager.PlayOneShot(deadSound);
    }

    void Attack()
    {
        battler.Attack();
        FMODUnity.RuntimeManager.PlayOneShot(attackSound);
    }

    private void Awake()
    {
        if (battler == null)
            battler = GetComponentInParent<Battler>();
    }
}
