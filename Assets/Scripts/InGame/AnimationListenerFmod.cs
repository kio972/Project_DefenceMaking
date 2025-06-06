using System.Collections;
using System.Collections.Generic;
using Spine;
using Spine.Unity;
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
        battler?.Attack();
        FMODUnity.RuntimeManager.PlayOneShot(attackSound, transform.position);
    }

    void SkillAttack()
    {
        battler?.Attack();
        FMODUnity.RuntimeManager.PlayOneShot(attackSound, transform.position);
    }

    void Recall()
    {
        FMODUnity.RuntimeManager.PlayOneShot(recallSound, transform.position);
    }

    private void HandleSpineEvent(TrackEntry trackEntry, Spine.Event e)
    {
        switch(e.Data.Name)
        {
            case "ATTACK":
                Attack();
                break;
        }
    }

    private void Awake()
    {
        if (battler == null)
            battler = GetComponentInParent<Battler>();

        var _skeletonAnimation = GetComponent<SkeletonAnimation>();
        if (_skeletonAnimation != null)
        {
            _skeletonAnimation.AnimationState.Event += HandleSpineEvent;
        }
    }
}
