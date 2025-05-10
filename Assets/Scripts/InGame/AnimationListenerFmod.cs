using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationListenerFmod : MonoBehaviour
{
    private Battler battler;

    [SerializeField]
    FMODUnity.EventReference attackSound;

    [Space]
    [SerializeField]
    FMODUnity.EventReference deadSound;
    [Space]
    [SerializeField]
    FMODUnity.EventReference skillSound;
    [Space]
    [SerializeField]
    FMODUnity.EventReference recallSound;
    void Dead()
    {
        FMODUnity.RuntimeManager.PlayOneShot(deadSound, transform.position);
    }

    void Attack()
    {
        battler.Attack();
        FMODUnity.RuntimeManager.PlayOneShot(attackSound, transform.position);
    }

    void SkillAttack()
    {
        battler.Attack();
        FMODUnity.RuntimeManager.PlayOneShot(recallSound, transform.position);
    }

    void Recall()
    {

    }

    private void Awake()
    {
        if (battler == null)
            battler = GetComponentInParent<Battler>();
    }
}
