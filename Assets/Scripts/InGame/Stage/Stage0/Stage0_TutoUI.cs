using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Stage0_TutoUI : MonoBehaviour
{
    [SerializeField]
    Stage0_Story story;
    private int _progress { get => story.tutoProgress; }

    [SerializeField]
    private GameObject rotateGuide;

    [SerializeField]
    private GameObject deployBtn;
    [SerializeField]
    private GameObject researchBtn;
    [SerializeField]
    private GameObject shopBtn;

    [SerializeField]
    private GameObject[] cardGuide;
    [SerializeField]
    private GameObject tileGuide;

    private readonly Vector3 crossRoadPos = Vector3.right * 4;
    private readonly Vector3 roomPos = new Vector3(4.5f, 0, -0.866f);

    [SerializeField]
    private DeployUI tutoDeploy;
    [SerializeField]
    private GameObject tutoDeployBtn;
    [SerializeField]
    private GameObject realDeploy;
    [SerializeField]
    private GameObject btnGuide;
    [SerializeField]
    private GameObject deployConfirmGuide;

    [SerializeField]
    private GameObject tutoCanvas;

    [SerializeField]
    private GameObject deployBlock;
    [SerializeField]
    private GameObject deployKeyOpenBlock;
    [SerializeField]
    private GameObject researchBlock;

    [SerializeField]
    private GameObject researchPage;
    [SerializeField]
    private ResearchMainUI researchMain;

    [SerializeField]
    private GameObject researchSlimeGuide;
    [SerializeField]
    private GameObject researchConfirmBtn;
    [SerializeField]
    private GameObject researchConfirmGuide;
    [SerializeField]
    private GameObject slimeResearchClicked;
    [SerializeField]
    private GameObject researchCloseGuide;
    [SerializeField]
    private GameObject[] otherResearchs;

    [SerializeField]
    private Button trapBtn;
    [SerializeField]
    private DeploySlot slimeSlot;
    [SerializeField]
    private GameObject deploySlimeGuide;
    [SerializeField]
    private SlotInfo deploySlotInfo;
    [SerializeField]
    private Button deployConfirmBtn;

    async UniTaskVoid Start()
    {
        if (SaveManager.Instance.playerData != null)
        {
            deployBtn.SetActive(true);
            researchBtn.SetActive(true);
            FinishTutoUI();
            return;
        }

        await UniTask.WaitUntil(() => _progress >= 1, cancellationToken: gameObject.GetCancellationTokenOnDestroy());
        rotateGuide.gameObject.SetActive(false);

        TileNode endTile = NodeManager.Instance.endPoint;
        NodeManager.Instance.SetActiveNode(endTile, false);

        await UniTask.WaitForSeconds(0.3f, cancellationToken: gameObject.GetCancellationTokenOnDestroy());
        while (_progress == 1)
        {
            int handCount = GameManager.Instance.cardDeckController.handCards.Count;
            for(int i = 0; i < 3; i++)
            {
                bool isActive = handCount > i && !InputManager.Instance.settingCard;
                cardGuide[i].SetActive(isActive);
                if(isActive)
                    cardGuide[i].transform.position = GameManager.Instance.cardDeckController.cards[i].transform.position;
            }

            tileGuide.SetActive(InputManager.Instance.settingCard);
            tileGuide.transform.position = Camera.main.WorldToScreenPoint(Vector3.right * (4 - handCount));

            await UniTask.Yield(cancellationToken: gameObject.GetCancellationTokenOnDestroy());
        }

        NodeManager.Instance.SetActiveNode(endTile, true);

        await UniTask.WaitUntil(() => _progress >= 2, cancellationToken: gameObject.GetCancellationTokenOnDestroy());
        rotateGuide.gameObject.SetActive(true);

        await UniTask.WaitForSeconds(0.3f, cancellationToken: gameObject.GetCancellationTokenOnDestroy());
        while (_progress == 2)
        {
            int handCount = GameManager.Instance.cardDeckController.handCards.Count;
            bool isActive = handCount > 0 && !InputManager.Instance.settingCard;
            cardGuide[0].SetActive(isActive);
            if (isActive)
                cardGuide[0].transform.position = GameManager.Instance.cardDeckController.cards[0].transform.position;

            tileGuide.SetActive(InputManager.Instance.settingCard);
            tileGuide.transform.position = Camera.main.WorldToScreenPoint(crossRoadPos);

            await UniTask.Yield(cancellationToken: gameObject.GetCancellationTokenOnDestroy());
        }

        await UniTask.WaitUntil(() => _progress >= 3, cancellationToken: gameObject.GetCancellationTokenOnDestroy());
        await UniTask.WaitForSeconds(0.5f, cancellationToken: gameObject.GetCancellationTokenOnDestroy());
        tutoDeployBtn.SetActive(true);
        deployKeyOpenBlock.SetActive(true);
        //deployBtn.SetActive(true);
        btnGuide.transform.position = deployBtn.transform.position;
        //tutoDeploy.enabled = false;
        while(_progress == 3 && GameManager.Instance.trapList.Count == 0)
        {
            btnGuide.SetActive(tutoDeploy.DeployStep == 0);
            deployConfirmGuide.SetActive(tutoDeploy.DeployStep == 1);
            tileGuide.SetActive(tutoDeploy.DeployStep == 2);
            tileGuide.transform.position = Camera.main.WorldToScreenPoint(Vector3.right * 2);
            await UniTask.Yield(cancellationToken: gameObject.GetCancellationTokenOnDestroy());
        }

        btnGuide.SetActive(false);
        tileGuide.SetActive(false);
        deployBlock.SetActive(true);
        deployKeyOpenBlock.SetActive(false);
        await UniTask.WaitUntil(() => _progress >= 4, cancellationToken: gameObject.GetCancellationTokenOnDestroy());
        await UniTask.WaitForSeconds(0.5f, cancellationToken: gameObject.GetCancellationTokenOnDestroy());
        Destroy(tutoDeployBtn.GetComponent<Animator>());
        //tutoDeployBtn.SetActive(false);

        researchBtn.SetActive(true);
        btnGuide.transform.position = researchBtn.transform.position;
        while (_progress == 4)
        {
            btnGuide.SetActive(!researchPage.activeSelf);
            researchSlimeGuide.SetActive(!slimeResearchClicked.activeSelf);
            researchConfirmGuide.SetActive(slimeResearchClicked.activeSelf && !researchConfirmBtn.activeSelf);
            researchCloseGuide.SetActive(researchConfirmBtn.activeSelf);

            await UniTask.Yield(cancellationToken: gameObject.GetCancellationTokenOnDestroy());
        }

        btnGuide.SetActive(false);
        researchBlock.SetActive(true);
        //researchPage.transform.parent.gameObject.SetActive(false);
        researchMain.enabled = false;
        //tutoDeploy.enabled = true;

        await UniTask.WaitUntil(() => _progress >= 5, cancellationToken: gameObject.GetCancellationTokenOnDestroy());

        await UniTask.WaitUntil(() => _progress >= 6, cancellationToken: gameObject.GetCancellationTokenOnDestroy());
        await UniTask.WaitForSeconds(0.3f, cancellationToken: gameObject.GetCancellationTokenOnDestroy());

        while (_progress == 6)
        {
            int handCount = GameManager.Instance.cardDeckController.handCards.Count;
            bool isActive = handCount > 0 && !InputManager.Instance.settingCard;
            cardGuide[0].SetActive(isActive);
            if (isActive)
                cardGuide[0].transform.position = GameManager.Instance.cardDeckController.cards[0].transform.position;

            tileGuide.SetActive(InputManager.Instance.settingCard);
            tileGuide.transform.position = Camera.main.WorldToScreenPoint(roomPos);

            await UniTask.Yield(cancellationToken: gameObject.GetCancellationTokenOnDestroy());
        }

        await UniTask.WaitUntil(() => _progress >= 7, cancellationToken: gameObject.GetCancellationTokenOnDestroy());
        btnGuide.transform.position = deployBtn.transform.position;
        deployBlock.SetActive(false);
        deployKeyOpenBlock.SetActive(true);
        tutoDeployBtn.SetActive(true);
        trapBtn.enabled = false;
        slimeSlot.gameObject.SetActive(true);
        while (_progress == 7)
        {
            btnGuide.SetActive(tutoDeploy.DeployStep == 0 && !researchPage.activeSelf);
            deploySlimeGuide.SetActive(deploySlotInfo.curDeploySlot != slimeSlot);
            deployConfirmGuide.SetActive(tutoDeploy.DeployStep == 1 && deploySlotInfo.curDeploySlot == slimeSlot);
            deployConfirmBtn.enabled = deploySlotInfo.curDeploySlot == slimeSlot;
            tileGuide.SetActive(tutoDeploy.DeployStep == 2);
            tileGuide.transform.position = Camera.main.WorldToScreenPoint(roomPos);
            await UniTask.Yield(cancellationToken: gameObject.GetCancellationTokenOnDestroy());

            await UniTask.Yield(cancellationToken: gameObject.GetCancellationTokenOnDestroy());
        }

        btnGuide.SetActive(false);
        tileGuide.SetActive(false);
        await UniTask.WaitUntil(() => _progress >= 10, cancellationToken: gameObject.GetCancellationTokenOnDestroy());

        FinishTutoUI();
    }

    private void FinishTutoUI()
    {
        Destroy(tutoCanvas);
        Destroy(researchCloseGuide);
        Destroy(researchConfirmGuide);
        Destroy(researchSlimeGuide);
        realDeploy.SetActive(true);
        //researchPage.transform.parent.gameObject.SetActive(true);
        researchMain.enabled = true;
    }
}
