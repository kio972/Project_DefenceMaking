using System.Collections;
using System.Collections.Generic;
using Spine.Unity;
using Spine;
using UniRx;
using UnityEngine;
using UnityEngine.SocialPlatforms;


[RequireComponent(typeof(SkeletonAnimation))]
public class SpineAnimationControl : MonoBehaviour, IBattlerAnimationController
{
    private SkeletonAnimation _skeletonAnimation;
    private SkeletonAnimation SkeletonAnim
    {
        get
        {
            if (_skeletonAnimation == null)
                _skeletonAnimation = GetComponent<SkeletonAnimation>();
            return _skeletonAnimation;
        }
    }

    private Spine.AnimationState State => SkeletonAnim.AnimationState;
    private Skeleton Skeleton => SkeletonAnim.Skeleton;

    public void SetMove(bool value) => State.SetAnimation(0, value ? "walk" : "idle", true);
    public void PlayAttackAnimation() => State.SetAnimation(0, "attack", false);

    public void UseSkill(bool value)
    {
        if (!value) return;
        State.SetAnimation(0, "skill", false);
    }

    public bool IsAttackEnd() => State.GetCurrent(0).IsComplete;

    public void UpdateMoveSpeed(float value) => SkeletonAnim.timeScale = value;
    public void UpdateAttackSpeed(float value) => SkeletonAnim.timeScale = value;

    public bool IsPlayingAnimation(string name, int layer = 0)
    {
        var entry = State.GetCurrent(layer);
        return entry != null && entry.Animation != null && entry.Animation.Name == name;
    }

    public void PlayAnimation(string name) => State.SetAnimation(0, name, false);

    public void ResetState()
    {
        // Animator.Rebind과 유사하게 Spine 상태 초기화
        State.ClearTracks();
        Skeleton.SetToSetupPose();
        SkeletonAnim.Initialize(true);
    }

    public void SetHide(bool value)
    {
        if (!value) return;
        State.SetAnimation(0, "hide", false);
    }

    public void Start()
    {
        Battler battler = GetComponentInParent<Battler>();
        if (battler == null)
            return;

        GameManager.Instance._timeScale.Subscribe(timeScale =>
        {
            
            if (battler.CurState == FSMPatrol.Instance || battler.CurState == FSMChase.Instance)
            {
                bool isMove = GameManager.Instance.timeScale != 0;
                SetMove(isMove);
                UpdateMoveSpeed(isMove ? battler.curMoveSpeed * timeScale : 1);
            }
            else if(battler.CurState != FSMDead.Instance)
                UpdateAttackSpeed(battler.curAttackSpeed * timeScale);

        }).AddTo(gameObject);

        battler._CurState.Where(state => state == FSMPatrol.Instance || state == FSMChase.Instance).Subscribe(_ =>
        {
            bool isMove = GameManager.Instance.timeScale != 0;
            SetMove(isMove);
            UpdateMoveSpeed(isMove ? battler.curMoveSpeed * GameManager.Instance.timeScale : 1);
        }).AddTo(gameObject);

        battler._CurState.Where(state => state == FSMAttack.Instance)
            .Subscribe(_ => UpdateAttackSpeed(battler.curAttackSpeed * GameManager.Instance.timeScale)).AddTo(gameObject);

        battler._CurState.Where(state => state == FSMDead.Instance)
            .Subscribe(_ => PlayAnimation("dead")).AddTo(gameObject);

        battler._effects.ObserveCountChanged().Subscribe(_ =>
        {
            if (battler.CurState == FSMPatrol.Instance || battler.CurState == FSMChase.Instance)
            {
                bool isMove = GameManager.Instance.timeScale != 0;
                SetMove(isMove);
                UpdateMoveSpeed(isMove ? battler.curMoveSpeed * GameManager.Instance.timeScale : 1);
            }
            else if (battler.CurState != FSMDead.Instance)
                UpdateAttackSpeed(battler.curAttackSpeed * GameManager.Instance.timeScale);
        }).AddTo(gameObject);
    }
}