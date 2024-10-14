using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillBtn : MonoBehaviour
{
    // Start is called before the first frame update
    async UniTaskVoid Start()
    {
        await UniTask.WaitUntil(() => GameManager.Instance.IsInit, cancellationToken: this.GetCancellationTokenOnDestroy());
        //await UniTask.WaitUntil(() => PassiveManager.Instance.UpgradeDevilAura, cancellationToken: this.GetCancellationTokenOnDestroy());
    }

    public void UseSkillBtn()
    {
        EmergencyEscape emergencyEscape = new EmergencyEscape();
        emergencyEscape.UseSkill();
    }
}
