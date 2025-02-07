using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DanStage : MonoBehaviour
{
    [SerializeField]
    List<GameObject> herbInformer;

    [SerializeField]
    List<GameObject> managementBtns;

    [SerializeField]
    AK.Wwise.Event ambient;
    [SerializeField]
    AK.Wwise.Event bgmSound;
    [SerializeField]
    AK.Wwise.Event battleSound;
    [SerializeField]
    AK.Wwise.Event battleSound2;

    private async UniTaskVoid CheckHerbInformer()
    {
        if (QuestManager.Instance.IsQuestEnded("q2002") || QuestManager.Instance.questController.subQuest.Where(_ => _._QuestID == "q2002").Count() >= 1)
            return;

        foreach(GameObject herb in herbInformer)
            herb.SetActive(false);

        await UniTask.WaitUntil(() => QuestManager.Instance.IsQuestEnded("q2002") || QuestManager.Instance.questController.subQuest.Where(_ => _._QuestID == "q2002").Count() >= 1,
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
        foreach(var btn in managementBtns)
            btn.SetActive(true);
        CheckHerbInformer().Forget();

        AudioManager.Instance.PlayBackground(bgmSound);
    }

    void Start()
    {
        ambient?.Post(gameObject);
        if (SaveManager.Instance.playerData == null)
            StartCoroutine(ITutorial());
        else
        {
            GameManager.Instance.LoadGame(SaveManager.Instance.playerData);
            CheckHerbInformer().Forget();
            foreach (var btn in managementBtns)
                btn.SetActive(true);
            AudioManager.Instance.PlayBackground(bgmSound);
        }
    }
}
