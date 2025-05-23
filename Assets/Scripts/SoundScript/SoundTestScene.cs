using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class SoundTestScene : MonoBehaviour
{
    [SerializeField]
    List<GameObject> targets;

    GameObject curTarget;
    int curIndex = 0;

    public void SetDie(bool value)
    {
        Animator animator = curTarget.GetComponentInChildren<Animator>();
        animator?.SetBool("Die", value);
    }

    public void SetSkill(bool value)
    {
        Animator animator = curTarget.GetComponentInChildren<Animator>();
        animator?.SetBool("Skill", value);
    }

    public void SetTrigger(string triggerName)
    {
        Animator animator = curTarget.GetComponentInChildren<Animator>();
        animator?.SetTrigger(triggerName);
        animator?.SetFloat("AttackSpeed", 1f);
    }

    public void SetMove()
    {
        Animator animator = curTarget.GetComponentInChildren<Animator>();
        animator?.SetBool("Move", !animator.GetBool("Move"));
    }

    public void LeftBtn()
    {
        curIndex--;
        if(curIndex < 0)
            curIndex = targets.Count - 1;


        curTarget.SetActive(false);
        curTarget = targets[curIndex];
        curTarget.SetActive(true);
    }

    public void RightBtn()
    {
        curIndex++;
        if (curIndex > targets.Count - 1)
            curIndex = 0;

        curTarget.SetActive(false);
        curTarget = targets[curIndex];
        curTarget.SetActive(true);
    }

    private void Start()
    {
        if(targets != null && targets.Count > 0)
            curTarget = targets[0];
    }
}
