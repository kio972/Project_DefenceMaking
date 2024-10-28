using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DanStage : MonoBehaviour
{
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
    }

    void Start()
    {
        if (SaveManager.Instance.playerData == null)
            StartCoroutine(ITutorial());
        else
            GameManager.Instance.LoadGame(SaveManager.Instance.playerData);
    }
}
