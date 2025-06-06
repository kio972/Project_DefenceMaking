using System.Collections;
using System.Collections.Generic;
using Spine.Unity;
using Spine;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;
using UniRx;
using UnityEngine.SocialPlatforms;

public interface IBattlerAnimationController
{
    public void ResetState();
    public void SetMove(bool value);
    public void PlayAttackAnimation();
    public void UseSkill(bool value);
    public void UpdateMoveSpeed(float value);
    public void UpdateAttackSpeed(float value);
    public bool IsAttackEnd();
    public bool IsPlayingAnimation(string name, int layer = 0);
    public void PlayAnimation(string name);

    public void SetHide(bool value);

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

    public void ResetState() => animator.Rebind();

    public void PlayAnimation(string name) => animator.SetTrigger(name);

    public void SetMove(bool value) => animator.SetBool("Move", value);
    public void PlayAttackAnimation() => animator.SetTrigger("Attack");
    public void UpdateMoveSpeed(float value) => animator.SetFloat("MoveSpeed", value);
    public void UpdateAttackSpeed(float value) => animator.SetFloat("AttackSpeed", value);

    public void UseSkill(bool value) => animator.SetBool("Skill", value);

    public bool IsAttackEnd()
    {
        bool isEnd = !animator.GetCurrentAnimatorStateInfo(0).IsTag("ATTACK") && !animator.IsInTransition(0);
        if(isEnd)
            animator.ResetTrigger("Attack");

        return isEnd;
    }

    public bool IsPlayingAnimation(string name, int layer) => animator.GetCurrentAnimatorStateInfo(layer).IsTag(name) && !animator.IsInTransition(layer);

    public void SetHide(bool value) => animator.SetBool("Hide", value);

    public void Start()
    {
        Battler battler = GetComponentInParent<Battler>();
        if (battler == null)
            return;

        GameManager.Instance._timeScale.Subscribe(timeScale =>
        {
            UpdateAttackSpeed(battler.curAttackSpeed * timeScale);
            if(battler.CurState == FSMPatrol.Instance || battler.CurState == FSMChase.Instance)
            {
                SetMove(GameManager.Instance.timeScale != 0);
                UpdateMoveSpeed(battler.curMoveSpeed * timeScale);
            }
        }).AddTo(gameObject);

        battler._CurState.Where(state => state == FSMPatrol.Instance || state == FSMChase.Instance).Subscribe(_ =>
        {
            SetMove(GameManager.Instance.timeScale != 0);
            UpdateMoveSpeed(battler.curMoveSpeed * GameManager.Instance.timeScale);
        }).AddTo(gameObject);

        battler._CurState.Where(state => state == FSMAttack.Instance)
            .Subscribe(_ => UpdateAttackSpeed(battler.curAttackSpeed * GameManager.Instance.timeScale)).AddTo(gameObject);

        battler._CurState.Where(state => state == FSMDead.Instance)
            .Subscribe(_ => animator.SetBool("Die", true)).AddTo(gameObject);

        battler._effects.ObserveCountChanged().Subscribe(_ =>
        {
            UpdateAttackSpeed(battler.curAttackSpeed * GameManager.Instance.timeScale);
            UpdateMoveSpeed(battler.curMoveSpeed * GameManager.Instance.timeScale);
        }).AddTo(gameObject);
    }
}