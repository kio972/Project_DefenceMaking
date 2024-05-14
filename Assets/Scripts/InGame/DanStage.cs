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
        QuestManager.Instance.Init();
    }

    void Start()
    {
        StartCoroutine(ITutorial());
    }
}
