using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ResearchProgressGlow : MonoBehaviour
{
    [SerializeField]
    ResearchMainUI researchPage;
    [SerializeField]
    GameObject researchUI;

    [SerializeField]
    Image fillImage;
    [SerializeField]
    GameObject glowImage;

    //private bool activeState = false;

    private CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

    private void OnDestroy()
    {
        cancellationTokenSource.Cancel();
        cancellationTokenSource.Dispose();
    }

    private async UniTaskVoid WatchResearchProgress()
    {
        while(true)
        {
            await UniTask.WaitUntil(() => researchPage.CurProgressRate != -1, default, cancellationTokenSource.Token);

            fillImage.fillAmount = 0;
            fillImage.gameObject.SetActive(true);

            float progressRate = researchPage.CurProgressRate;
            while ((progressRate = researchPage.CurProgressRate) != -1)
            {
                fillImage.fillAmount = progressRate;
                await UniTask.Yield(cancellationTokenSource.Token);
            }

            fillImage.fillAmount = 1;
            glowImage.SetActive(true);

            await UniTask.WaitUntil(() => researchUI.activeSelf, default, cancellationTokenSource.Token);
            glowImage.SetActive(false);
            fillImage.gameObject.SetActive(false);
        }
    }

    public void Start()
    {
        if (researchPage == null)
            return;

        WatchResearchProgress().Forget();
        glowImage.SetActive(false);
    }
}
