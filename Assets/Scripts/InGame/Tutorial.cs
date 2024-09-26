using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Tutorial : MonoBehaviour
{
    [SerializeField]
    private GameObject tuto1;

    [SerializeField]
    private GameObject topBlocker;
    [SerializeField]
    private GameObject rightBlocker;
    [SerializeField]
    private GameObject downBlocker;

    [SerializeField]
    TutoHelpBtn helpBtn;

    [SerializeField]
    private DeployUI managePage;

    [SerializeField]
    private Transform manageBtnPos;
    [SerializeField]
    private Transform shopBtnPos;
    [SerializeField]
    private Transform researchBtnPos;
    [SerializeField]
    private Transform speed1Btn;

    [SerializeField]
    private Transform deckBtnPos;

    [SerializeField]
    private GameObject researchUI;
    [SerializeField]
    private GameObject researchPopUp;
    [SerializeField]
    private ResearchSlot tutoSlimeSlot;
    [SerializeField]
    private ResearchSlot slimeSlot;

    [SerializeField]
    private GameObject arrowUp;

    [SerializeField]
    private GameObject delpoyArrow;
    [SerializeField]
    private GameObject delpoyArrow2;
    [SerializeField]
    private GameObject[] pathArrowGroup;
    [SerializeField]
    private GameObject delpoyExitArrow;
    [SerializeField]
    private GameObject delpoyExitArrow2;

    [SerializeField]
    private GameObject researchSlotArrow;
    [SerializeField]
    private GameObject researchPopupArrow;
    [SerializeField]
    private GameObject researchExitArrow;

    [SerializeField]
    private GameObject arrowDown;
    [SerializeField]
    private GameObject arrowRight;
    [SerializeField]
    private GameObject arrowRight2;

    [SerializeField]
    private GameObject trapDeploySlot;
    [SerializeField]
    private DeploySlot slimeDeploySlot;

    [SerializeField]
    private InGameUI ingameUI;

    [SerializeField]
    private TextMeshProUGUI rotationInfo;

    private void SetHelpBtn(bool value, Vector3 position = new Vector3(), int stage = -1)
    {
        helpBtn.gameObject.SetActive(value);
        if(value)
        {
            helpBtn.stage = stage;
            helpBtn.transform.position = position;
        }
    }

    //private void SetEndTileMoveGuide(Vector3 targetPos, Vector3 endPos)
    //{
    //    if (InputManager.Instance._CurTile == NodeManager.Instance.endPoint.curTile)
    //    {
    //        if (InputManager.Instance.movingTile)
    //        {
    //            arrowDown.SetActive(true);
    //            arrowDown.transform.position = Camera.main.WorldToScreenPoint(targetPos);
    //            arrowRight2.gameObject.SetActive(false);
    //            rotationInfo.gameObject.SetActive(true);
    //        }
    //        else
    //        {
    //            arrowDown.SetActive(false);
    //            arrowRight2.gameObject.SetActive(true);
    //            rotationInfo.gameObject.SetActive(false);
    //        }
    //    }
    //    else
    //    {
    //        arrowDown.transform.position = Camera.main.WorldToScreenPoint(endPos);
    //        arrowDown.gameObject.SetActive(true);
    //        arrowRight2.gameObject.SetActive(false);
    //        rotationInfo.gameObject.SetActive(false);
    //    }
    //}

    private IEnumerator Dan001()
    {
        GameManager.Instance.mapBuilder.Init();
        GameManager.Instance.SpawnKing();

        ingameUI?.Init();
        ingameUI.SetRightUI(false, 0.1f);
        ingameUI.rightUILock = true;
        GameManager.Instance.isPause = true;
        GameManager.Instance.speedLock = true;
        NodeManager.Instance.SetGuideState(GuideState.None);

        yield return new WaitForSeconds(1f);

        StoryManager.Instance.EnqueueScript("Dan001");
        while (!StoryManager.Instance.IsScriptQueueEmpty)
            yield return null;

        Vector3 endPos = NodeManager.Instance.endPoint.transform.position;
        Vector3 targetPos = NodeManager.Instance.FindNode(-1, 5).transform.position;

        while (true)
        {
            //SetEndTileMoveGuide(targetPos, endPos);

            if ((endPos - NodeManager.Instance.endPoint.transform.position).magnitude > 0.2f)
                break;
            yield return null;
        }
        arrowDown.gameObject.SetActive(false);
        GameManager.Instance.tileLock = true;
    }

    private IEnumerator Dan002()
    {
        StoryManager.Instance.EnqueueScript("Dan002");
        while (!StoryManager.Instance.IsScriptQueueEmpty)
            yield return null;

        tuto1.SetActive(true);
        GameManager.Instance.isPause = true;
        while (tuto1.activeSelf)
            yield return null;
        GameManager.Instance.isPause = false;

        topBlocker.SetActive(false);
        downBlocker.SetActive(false);
        
        GameManager.Instance.speedLock = false;
        //GameManager.Instance.ForceInit();

    }

    private IEnumerator Dan003()
    {
        GameManager.Instance.waveController.SpawnWave(0);
        GameManager.Instance.spawnLock = true;

        while (GameManager.Instance.adventurersList.Count == 0)
            yield return null;

        //모험가 침입
        Vector3 prevPos = GameManager.Instance.cameraController._GuidePos;
        GameManager.Instance.speedController.SetSpeedZero();
        GameManager.Instance.spawnLock = true;
        GameManager.Instance.cameraController.CamMoveToPos(NodeManager.Instance.startPoint.transform.position);
        GameManager.Instance.cameraController.SetCamZoom(3);
        topBlocker.SetActive(true);

        StoryManager.Instance.EnqueueScript("Dan003");
        while (!StoryManager.Instance.IsScriptQueueEmpty)
            yield return null;
        GameManager.Instance.cameraController.CamMoveToPos(prevPos);
        GameManager.Instance.cameraController.SetCamZoom(1);
        GameManager.Instance.speedLock = true;
        arrowUp.transform.position = manageBtnPos.position;
        arrowUp.SetActive(true);
        SetHelpBtn(true, manageBtnPos.position, 0);

        Tile[] tiles = FindObjectsByType<Tile>(FindObjectsSortMode.None);
        List<Tile> pathTiles = new List<Tile>();
        foreach(Tile tile in tiles)
        {
            if (tile._TileType == TileType.Path)
                pathTiles.Add(tile);
        }

        while(GameManager.Instance.trapList.Count == 0)
        {
            arrowUp.SetActive(managePage.DeployStep == 0);
            for(int i = 0; i < pathTiles.Count; i++)
            {
                pathArrowGroup[i].SetActive(managePage.DeployStep == 2);
                if(pathArrowGroup[i].activeSelf)
                    pathArrowGroup[i].transform.position = Camera.main.WorldToScreenPoint(pathTiles[i].transform.position);
            }
            yield return null;
        }

        SetHelpBtn(false);
        delpoyArrow.SetActive(false);
        foreach (GameObject arrow in pathArrowGroup)
            arrow.SetActive(false);

        while(managePage.DeployStep != 0)
        {
            delpoyExitArrow.SetActive(managePage.DeployStep == 2);
            delpoyExitArrow2.SetActive(managePage.DeployStep == 1);
            yield return null;
        }
        delpoyExitArrow.SetActive(false);
        delpoyExitArrow2.SetActive(false);
        yield return StartCoroutine(WaitForSpeed1());
    }

    private IEnumerator Dan004()
    {
        while (GameManager.Instance.adventurersList.Count != 0)
            yield return null;

        GameManager.Instance.speedController.SetSpeedZero();

        StoryManager.Instance.EnqueueScript("Dan004");
        while (!StoryManager.Instance.IsScriptQueueEmpty)
            yield return null;

        GameManager.Instance.speedLock = true;
        SetHelpBtn(true, researchBtnPos.position, 1);
        arrowUp.transform.position = researchBtnPos.position;
        while(tutoSlimeSlot._CurState != ResearchState.InProgress)
        {
            arrowUp.gameObject.SetActive(!researchUI.activeSelf);
            researchSlotArrow.SetActive(!researchPopUp.activeSelf);
            yield return null;
        }

        researchPopupArrow.SetActive(false);
        researchExitArrow.SetActive(true);
        while (researchUI.activeSelf)
            yield return null;
        
        SetHelpBtn(false);

        yield return StartCoroutine(WaitForSpeed1());
    }

    private IEnumerator Dan005()
    {
        //while (true)
        //{
        //    if (PassiveManager.Instance.deployAvailableTable.ContainsKey("s_m10001") && PassiveManager.Instance.deployAvailableTable["s_m10001"])
        //        break;
        //    yield return null;
        //}
        //if (GameManager.Instance.gold < 50)
        //    GameManager.Instance.gold = 50;

        //slimeSlot.SetResearchState(ResearchState.Complete);
        //int slimeIndex = UtilHelper.Find_Data_Index("s_m10001", DataManager.Instance.Battler_Table, "id");
        //slimeDeploySlot.Init(DataManager.Instance.Battler_Table[slimeIndex]);
        //slimeDeploySlot.SendInfo();
        //slimeDeploySlot.gameObject.SetActive(true);

        //GameManager.Instance.speedController.SetSpeedZero();
        //GameManager.Instance.speedLock = true;
        //yield return new WaitForSeconds(1.5f);

        //StoryManager.Instance.EnqueueScript("Dan005");
        //while (!StoryManager.Instance.IsScriptQueueEmpty)
        //    yield return null;

        //ingameUI.SetRightUI(true, 0.1f);
        //ingameUI.rightUILock = false;
        //rightBlocker.SetActive(true);

        //yield return new WaitForSeconds(0.5f);

        //RectTransform rect = helpBtn.GetComponent<RectTransform>();
        //rect.pivot = new Vector2(0.5f, 0.5f);
        //SetHelpBtn(true, deckBtnPos.position, 2);
        //pathArrowGroup[0].SetActive(true);
        //pathArrowGroup[0].transform.position = deckBtnPos.position;

        //while (GameManager.Instance.cardDeckController.hand_CardNumber == 0)
        //    yield return null;
        //pathArrowGroup[0].SetActive(false);
        //yield return new WaitForSeconds(0.5f);

        //Vector3 targetPos = NodeManager.Instance.FindNode(1, 5).transform.position;

        //Transform roomTransform = GameManager.Instance.cardDeckController.cards[0];
        //pathArrowGroup[1].SetActive(true);
        //while (!NodeManager.Instance.HaveSingleRoom)
        //{
        //    if (!InputManager.Instance.settingCard)
        //        pathArrowGroup[1].transform.position = roomTransform.position;
        //    else
        //        pathArrowGroup[1].transform.position = Camera.main.WorldToScreenPoint(targetPos);

        //    yield return null;
        //}

        //GameManager.Instance.speedLock = true;
        //pathArrowGroup[1].SetActive(false);
        
        //Button[] btns = trapDeploySlot.GetComponentsInChildren<Button>(true);
        //foreach (Button btn in btns)
        //    Destroy(btn);
        //yield return null;

        //rect.pivot = new Vector2(0.5f, 1f);
        //SetHelpBtn(true, manageBtnPos.position, 0);
        //arrowUp.transform.position = manageBtnPos.position;
        //while (GameManager.Instance.monsterSpawner.Count == 0)
        //{
        //    arrowUp.SetActive(managePage.DeployStep == 0);
        //    pathArrowGroup[1].SetActive(managePage.DeployStep == 2);
        //    if(pathArrowGroup[1].activeSelf)
        //        pathArrowGroup[1].transform.position = Camera.main.WorldToScreenPoint(targetPos);
        //    yield return null;
        //}

        //SetHelpBtn(false);
        //delpoyArrow2.SetActive(false);
        //pathArrowGroup[1].SetActive(false);

        //while (managePage.DeployStep != 0)
        //{
        //    delpoyExitArrow.SetActive(managePage.DeployStep == 2);
        //    delpoyExitArrow2.SetActive(managePage.DeployStep == 1);
        //    yield return null;
        //}

        yield return StartCoroutine(WaitForSpeed1());
    }

    private IEnumerator Dan006()
    {
        while(GameManager.Instance._MonsterList.Count == 0)
            yield return null;

        Vector3 prevPos = GameManager.Instance.cameraController._GuidePos;
        GameManager.Instance.speedController.SetSpeedZero();
        GameManager.Instance.speedLock = true;
        GameManager.Instance.cameraController.CamMoveToPos(GameManager.Instance._MonsterList[0].transform.position);
        GameManager.Instance.cameraController.SetCamZoom(3);
        
        StoryManager.Instance.EnqueueScript("Dan006");
        while (!StoryManager.Instance.IsScriptQueueEmpty)
            yield return null;
        GameManager.Instance.cameraController.CamMoveToPos(prevPos);
        GameManager.Instance.cameraController.SetCamZoom(1);

        yield return new WaitForSeconds(1);
        GameManager.Instance.speedLock = false;
        GameManager.Instance.spawnLock = false;
        GameManager.Instance.cardLock = false;
        GameManager.Instance.tileLock = false;
        GameManager.Instance.mapBuilder.SetRamdomTile(4, 5);
        GameManager.Instance.cardDeckController.Mulligan();
        GameManager.Instance.SkipDay();
    }

    private IEnumerator WaitForSpeed1()
    {
        GameManager.Instance.speedLock = false;
        arrowUp.transform.position = speed1Btn.transform.position;
        while (GameManager.Instance.timeScale == 0)
        {
            arrowUp.gameObject.SetActive(true);
            yield return null;
        }
        arrowUp.gameObject.SetActive(false);
    }

    private IEnumerator ITutorial()
    {
        yield return null;
        StoryManager.Instance.EnqueueScript("Dan000");

        while(!StoryManager.Instance.IsScriptQueueEmpty)
            yield return null;

        yield return StartCoroutine(Dan001());

        yield return StartCoroutine(Dan002());

        yield return StartCoroutine(Dan003());

        yield return StartCoroutine(Dan004());

        yield return StartCoroutine(Dan005());

        yield return StartCoroutine(Dan006());

        Destroy(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(ITutorial());
    }
}
