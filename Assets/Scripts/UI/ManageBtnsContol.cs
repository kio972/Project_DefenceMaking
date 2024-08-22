using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using UnityEngine;

public class ManageBtnsContol : MonoBehaviour
{
    [SerializeField]
    private GameObject deployBtn;
    [SerializeField]
    private GameObject researchBtn;
    [SerializeField]
    private GameObject shopBtn;

    private readonly string deployQuestId = "q2001";
    private readonly string researchQuestId = "q2005";
    private readonly string shopQuestId = "q2007";

    public async UniTaskVoid WaitForQuest(GameObject target, string questId)
    {
        await UniTask.WaitUntil(() => QuestManager.Instance.questController.IsQuestStarted(questId));
        target.SetActive(true);
    }

    public async UniTaskVoid Start()
    {
        deployBtn.SetActive(false);
        researchBtn.SetActive(false);
        shopBtn.SetActive(false);

        await UniTask.WaitUntil(() => GameManager.Instance.IsInit);

        if (QuestManager.Instance.IsQuestClear(deployQuestId) || QuestManager.Instance.questController.IsQuestStarted(deployQuestId))
        {
            deployBtn.GetComponent<Animator>().enabled = false;
            deployBtn.gameObject.SetActive(true);
        }
        else
            WaitForQuest(deployBtn, deployQuestId).Forget();

        if (QuestManager.Instance.IsQuestClear(researchQuestId) || QuestManager.Instance.questController.IsQuestStarted(researchQuestId))
        {
            researchBtn.GetComponent<Animator>().enabled = false;
            researchBtn.gameObject.SetActive(true);
        }
        else
            WaitForQuest(researchBtn, researchQuestId).Forget();

        if (QuestManager.Instance.IsQuestClear(shopQuestId) || QuestManager.Instance.questController.IsQuestStarted(shopQuestId))
        {
            shopBtn.GetComponent<Animator>().enabled = false;
            shopBtn.gameObject.SetActive(true);
        }
        else
            WaitForQuest(shopBtn, shopQuestId).Forget();

    }
}
