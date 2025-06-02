using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBattlerAnimationController
{
    public void Rebind();
    public void SetBool(string name, bool value);
    public void SetTrigger(string name);
    public void ResetTrigger(string name);
    public void SetFloat(string name, float value);
    public bool ContainsParam(string name);
    public bool GetCurrentAnimatorStateInfo(string tag, int layer = 0);
    public bool IsInTransition(int layer = 0);
}

[RequireComponent(typeof(Animator))]
public class UnityAnimationControl : MonoBehaviour, IBattlerAnimationController
{
    Animator _animator;
    Animator animator
    {
        get
        {
            if(_animator == null)
                _animator = GetComponent<Animator>();
            return _animator;
        }
    }

    public void Rebind() => animator.Rebind();
    public void SetBool(string name, bool value) => animator.SetBool(name, value);
    public void SetTrigger(string name) => animator.SetTrigger(name);
    public void ResetTrigger(string name) => animator.ResetTrigger(name);
    public void SetFloat(string name, float value) => animator.SetFloat(name, value);
    public bool ContainsParam(string name) => animator.ContainsParam(name);

    public bool GetCurrentAnimatorStateInfo(string tag, int layer = 0) => animator.GetCurrentAnimatorStateInfo(layer).IsTag(tag); 
    public bool IsInTransition(int layer = 0) => animator.IsInTransition(layer);
}