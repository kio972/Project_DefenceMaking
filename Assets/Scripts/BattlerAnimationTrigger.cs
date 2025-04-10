using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class BattlerAnimationTrigger : MonoBehaviour
{
    [SerializeField]
    Animator targetAnimator;

    public void SetTrigger(string targetName)
    {
        targetAnimator.SetTrigger(targetName);
    }

    public void SetMove(bool value)
    {
        targetAnimator.SetBool("Move", value);
    }

    public void SetActiveBool(string targetName)
    {
        targetAnimator.SetBool(targetName, true);
    }

    public void SetDeActiveBool(string targetName)
    {
        targetAnimator.SetBool(targetName, false);
    }
}
