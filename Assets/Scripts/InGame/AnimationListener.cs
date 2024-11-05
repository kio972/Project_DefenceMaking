using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationListener : MonoBehaviour
{
    private Battler battler;

    [SerializeField]
    private AK.Wwise.Event attackSounds;
    [Space]
    [SerializeField]
    private AK.Wwise.Event deadSounds;

    void Dead()
    {
        deadSounds?.Post(gameObject);
    }

    void Attack()
    {
        battler?.Attack();
        attackSounds?.Post(gameObject);
    }

    private void Awake()
    {
        if (battler == null)
            battler = GetComponentInParent<Battler>();
    }
}
