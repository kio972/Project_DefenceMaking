using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class SkillController : MonoBehaviour
{
    private List<ISkill> skills = new List<ISkill>();

    [SerializeField]
    private SkillBtn skillPrefab;

    private List<SkillBtn> skillSlots = new List<SkillBtn>();

    private void AddSkill(ISkill skill)
    {
        if (skill.IsPassive)
            return;

        SkillBtn newSkill = Instantiate(skillPrefab, transform);
        newSkill.Init(skill);
        skillSlots.Add(newSkill);
        newSkill.gameObject.SetActive(true);
    }

    private async UniTaskVoid Start()
    {
        skillPrefab.gameObject.SetActive(false);
        await UniTask.WaitUntil(() => GameManager.Instance.IsInit, cancellationToken: this.GetCancellationTokenOnDestroy());
        Battler battler = GameManager.Instance.king;
        if(battler is PlayerBattleMain devil)
        {
            foreach(ISkill skill in devil.skills)
                AddSkill(skill);

            devil.skills.ObserveAdd().Subscribe(_ => AddSkill(_.Value)).AddTo(gameObject);
        }
    }
}
