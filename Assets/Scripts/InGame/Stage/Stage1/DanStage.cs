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

    //[SerializeField]
    //AK.Wwise.Event ambient;
    //[SerializeField]
    //AK.Wwise.Event bgmSound;
    //[SerializeField]
    //AK.Wwise.Event battleSound;
    //[SerializeField]
    //AK.Wwise.Event battleSound2;

    public bool bossEntered { get; private set; } = false;


    readonly string shopOpenQuestId = "q2005";
    readonly string vicenteQuestId = "q2006";

    private async UniTaskVoid SetManageBtns()
    {
        for(int i = 0; i < 2; i++)
        {
            managementBtns[i].SetActive(true);
        }

        await UniTask.WaitUntil(() => QuestManager.Instance.questController.subQuest.Where(_ => _._QuestID == shopOpenQuestId).Count() >= 1 || QuestManager.Instance.IsQuestEnded(shopOpenQuestId),
            cancellationToken: gameObject.GetCancellationTokenOnDestroy());

        managementBtns[2].SetActive(true);
    }

    private async UniTaskVoid PlayBossBattleBGM()
    {
        //AudioManager.Instance.PlayBackground(battleSound);
        await UniTask.WaitUntil(() => QuestManager.Instance.IsQuestCleared(vicenteQuestId), cancellationToken: gameObject.GetCancellationTokenOnDestroy());
        //AudioManager.Instance.PlayBackground(bgmSound);
    }

    private async UniTaskVoid CheckBossEnter()
    {
        if (QuestManager.Instance.IsQuestEnded(vicenteQuestId))
            return;

        if (QuestManager.Instance.questController.subQuest.Where(_ => _._QuestID == vicenteQuestId).Count() >= 1)
        {
            PlayBossBattleBGM().Forget();
            return;
        }

        await UniTask.WaitUntil(() => GameManager.Instance.CurWave >= 29, cancellationToken: gameObject.GetCancellationTokenOnDestroy());

        StoryManager.Instance.EnqueueScript("Dan001");

        await UniTask.WaitUntil(() => StoryManager.Instance.IsScriptQueueEmpty, cancellationToken: gameObject.GetCancellationTokenOnDestroy());
        bossEntered = true;

        PlayBossBattleBGM().Forget();
    }

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
        SetManageBtns().Forget();
        CheckHerbInformer().Forget();
        CheckBossEnter().Forget();

        //AudioManager.Instance.PlayBackground(bgmSound);
    }

    void Start()
    {
        //ambient?.Post(gameObject);
        if (SaveManager.Instance.playerData == null)
            StartCoroutine(ITutorial());
        else
        {
            GameManager.Instance.LoadGame(SaveManager.Instance.playerData);
            CheckHerbInformer().Forget();
            CheckBossEnter().Forget();
            SetManageBtns().Forget();
            //AudioManager.Instance.PlayBackground(bgmSound);
        }
    }
}
