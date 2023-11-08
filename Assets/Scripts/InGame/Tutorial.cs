using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial : MonoBehaviour
{
    private bool waitForStart = false;

    [SerializeField]
    private GameObject tuto1;

    [SerializeField]
    private GameObject topBlocker;
    [SerializeField]
    private GameObject rightBlocker;
    [SerializeField]
    private GameObject downBlocker;

    [SerializeField]
    private GameObject managePage;

    [SerializeField]
    private GameObject arrowDown;
    [SerializeField]
    private GameObject arrowRight;

    private IEnumerator ITutorial()
    {
        yield return null;
        StoryManager.Instance.EnqueueScript("Dan000");

        while(!StoryManager.Instance.IsScriptQueueEmpty)
            yield return null;

        GameManager.Instance.Init();
        Transform roomTransform = GameManager.Instance.cardDeckController.cards[3];
        GameManager.Instance.isPause = true;
        GameManager.Instance.speedLock = true;
        yield return new WaitForSeconds(2f);

        StoryManager.Instance.EnqueueScript("Dan001");
        while (!StoryManager.Instance.IsScriptQueueEmpty)
            yield return null;

        Vector3 endPos = NodeManager.Instance.endPoint.transform.position;
        while (true)
        {
            arrowDown.transform.position = Camera.main.WorldToScreenPoint(endPos);
            arrowDown.gameObject.SetActive(!InputManager.Instance.movingTile);
            if ((endPos - NodeManager.Instance.endPoint.transform.position).magnitude > 0.2f)
                break;
            yield return null;
        }
        arrowDown.gameObject.SetActive(false);

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

        while (GameManager.Instance.adventurersList.Count == 0)
            yield return null;

        GameManager.Instance.speedLock = true;
        GameManager.Instance.cameraController.CamMoveToPos(NodeManager.Instance.startPoint.transform.position);
        GameManager.Instance.cameraController.SetCamZoom(3);
        topBlocker.SetActive(true);
        GameManager.Instance.speedLock = true;
        StoryManager.Instance.EnqueueScript("Dan003");
        while (!StoryManager.Instance.IsScriptQueueEmpty)
            yield return null;

        GameManager.Instance.cameraController.SetCamZoom(1);
        GameManager.Instance.speedController.SetSpeedZero();
        
        if(!NodeManager.Instance.HaveSingleRoom)
        {
            arrowDown.SetActive(true);
            arrowDown.transform.position = roomTransform.position + (Vector3.up * 100);
        }

        while (!NodeManager.Instance.HaveSingleRoom)
        {
            arrowDown.gameObject.SetActive(!InputManager.Instance.settingCard);
            yield return null;
        }

        arrowRight.gameObject.SetActive(true);
        while (!managePage.activeSelf)
            yield return null;

        arrowRight.gameObject.SetActive(false);
        topBlocker.SetActive(false);
        GameManager.Instance.speedLock = false;

        while (GameManager.Instance.adventurersList.Count != 0)
            yield return null;

        StoryManager.Instance.EnqueueScript("Dan004");
        
        while (!StoryManager.Instance.IsScriptQueueEmpty)
            yield return null;

        GameManager.Instance.mapBuilder.SetRamdomTile(4, 5);
        GameManager.Instance.SkipDay();
        //튜토리얼 끝날 시

        while (GameManager.Instance.cardDeckController.hand_CardNumber != 10)
            yield return null;

        StoryManager.Instance.EnqueueScript("Dan005");
    }

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(ITutorial());
    }

    // Update is called once per frame
    void Update()
    {

    }
}
