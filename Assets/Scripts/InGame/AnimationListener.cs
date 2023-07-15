using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationListener : MonoBehaviour
{
    [SerializeField]
    private Battler battler;

    void Attack()
    {
        battler.Attack();
    }
}
