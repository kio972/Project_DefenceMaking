using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DanStage : MonoBehaviour
{
    [SerializeField]
    List<GameObject> herbInformer;

    private async UniTaskVoid CheckHerbInformer()
    {
        if (QuestManager.Instance.IsQuestClear("q2002") || QuestManager.Instance.questController.subQuest.Where(_ => _._QuestID == "q2002").Count() >= 1)
            return;

        foreach(GameObject herb in herbInformer)
            herb.SetActive(false);

        await UniTask.WaitUntil(() => QuestManager.Instance.IsQuestClear("q2002") || QuestManager.Instance.questController.subQuest.Where(_ => _._QuestID == "q2002").Count() >= 1,
            cancellationToken: gameObject.GetCancellationTokenOnDestroy());

        foreach(GameObject herb in herbInformer)
            herb.SetActive(true);
    }

    private IEnumerator ITutorial()
    {
        yield return null;
        StoryManager.Instance.EnqueueScript("Dan000");
        while (!StoryManager.Instance.IsScriptQueueEmpty)
            yield return null;

        GameManager.Instance.Init();
        GameManager.Instance.cardDeckController.Invoke("MulliganFixed", 1f);
        GameManager.Instance.research.ForceActiveResearch("r_m10001");
        QuestManager.Instance.InitQuest();
        CheckHerbInformer().Forget();
    }

    void Start()
    {
        if (SaveManager.Instance.playerData == null)
            StartCoroutine(ITutorial());
        else
        {
            GameManager.Instance.LoadGame(SaveManager.Instance.playerData);
            CheckHerbInformer().Forget();
        }
    }
}
