using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileAnimationActivator : MonoBehaviour
{
    private void OnEnable()
    {
        Animator animator = GetComponentInChildren<Animator>();
        animator?.SetTrigger("Set");
    }
}
