using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum CardType
{
    MapTile,
    Monster,
    Trap,
}

public class CardController : MonoBehaviour
{
    public CardType cardType;
    [SerializeField]
    private GameObject targetPrefab;

    private Button button;

    private GameObject instancedObject;

    private Collider targetCollider;

    private TileNode curNode = null;

    private bool SetInput()
    {
        if(Input.GetKeyDown(KeyCode.Mouse0))
        {
            return true;
        }

        return false;
    }

    private void Discard(bool value)
    {
        if(value)
        {
            //타일 놓기
            instancedObject = null;
            GameManager.Instance.cardDeckController.hand_CardNumber--;
            Destroy(this.gameObject);
        }
        else
        {
            instancedObject.SetActive(false);
        }

        if(instancedObject != null)
        {
            Destroy(instancedObject.gameObject);
            instancedObject = null;
        }
        curNode = null;
    }

    private void SetObjectOnMap()
    {
        bool discard = false;
        if(curNode != null)
        {
            discard = true;
            TileNode tileNode = instancedObject.GetComponent<TileNode>();
            if(tileNode != null)
            {
                tileNode.SwapTile(curNode);
                NodeManager.Instance.emptyNodes.Remove(curNode);
                NodeManager.Instance.activeNodes.Add(tileNode);
                tileNode.transform.position = curNode.transform.position;
                Destroy(curNode.gameObject);
            }

            Trap trap = instancedObject.GetComponent<Trap>();
            if(trap != null)
            {
                trap.transform.SetParent(curNode.transform);
                trap.Init();
            }
        }

        Discard(discard);
        InputManager.Instance.settingCard = false;
        NodeManager.Instance.ResetAvail();
    }

    private void UpdateObjectPosition()
    {
        curNode = UtilHelper.RayCastTile();

        if(curNode != null)
        {
            instancedObject.transform.position = curNode.transform.position;
        }
        else
        {
            instancedObject.transform.position = new Vector3(0, 10000, 0);
        }
    }

    private void UpdateObjectState()
    {
        UpdateObjectPosition();
        if(cardType == CardType.MapTile && Input.GetKeyDown(KeyCode.R))
        {
            TileNode tile = instancedObject.GetComponent<TileNode>();
            tile.RotateTile();
            ReadyForMapTile();
        }

        if (SetInput())
            SetObjectOnMap();
    }

    private void ReadyForTrap()
    {
        UtilHelper.SetCollider(false, NodeManager.Instance.emptyNodes);
        UtilHelper.SetAvail(true, NodeManager.Instance.activeNodes);
        UtilHelper.SetCollider(true, NodeManager.Instance.activeNodes);
    }

    private void ReadyForMapTile()
    {
        UtilHelper.SetCollider(false, NodeManager.Instance.emptyNodes);
        UtilHelper.SetCollider(false, NodeManager.Instance.activeNodes);
        NodeManager.Instance.SetVirtualTile();
        TileNode tileNode = instancedObject.GetComponent<TileNode>();
        NodeManager.Instance.SetPathAvailTile(tileNode);
    }

    private void CallCard()
    {
        //targetPrefab생성
        if(instancedObject == null)
            instancedObject = Instantiate(targetPrefab);
        else
            instancedObject.SetActive(true);
        targetCollider = instancedObject.GetComponentInChildren<Collider>();
        if(targetCollider != null ) { targetCollider.enabled = false; }
        InputManager.Instance.settingCard = true;
        //카드 종류별 처리
        switch (cardType)
        {
            case CardType.MapTile:
                ReadyForMapTile();
                break;
            case CardType.Monster:
                ReadyForTrap();
                break;
            case CardType.Trap:
                ReadyForTrap();
                break;
        }
    }

    public void Init(CardType type, GameObject targetPrefab)
    {
        this.cardType = type;
        this.targetPrefab = targetPrefab;
        button = GetComponent<Button>();
        button.onClick.AddListener(CallCard);
    }

    // Update is called once per frame
    void Update()
    {
        if (instancedObject != null && instancedObject.gameObject.activeSelf)
            UpdateObjectState();
    }

    private void Start()
    {
        Init(cardType, targetPrefab);
    }
}
