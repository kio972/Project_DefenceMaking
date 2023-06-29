using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public enum CardType
{
    MapTile,
    Monster,
    Trap,
}

public class CardController : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    public CardType cardType;
    [SerializeField]
    private GameObject targetPrefab;

    private Button button;

    private GameObject instancedObject;

    private Collider targetCollider;

    private TileNode curNode = null;

    private TileNode availNodes;

    private Coroutine scale_Modify_Coroutine = null;
    private Coroutine position_Modify_Coroutine = null;

    private bool drawEnd = false;

    public Vector3 originPos;

    [SerializeField]
    private float mouseOverTime = 0.2f;

    private void DrawEnd()
    {
        drawEnd = true;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!drawEnd)
            return;

        if (scale_Modify_Coroutine != null)
            StopCoroutine(scale_Modify_Coroutine);
        scale_Modify_Coroutine = StartCoroutine(IScaleEffect(transform.localScale, Vector3.one * 1.2f, mouseOverTime));

        if (position_Modify_Coroutine != null)
            StopCoroutine(position_Modify_Coroutine);
        position_Modify_Coroutine = StartCoroutine(IMoveEffect(transform.position, new Vector3(originPos.x, 150, originPos.z), mouseOverTime));
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        if (!drawEnd)
            return;

        if (scale_Modify_Coroutine != null)
            StopCoroutine(scale_Modify_Coroutine);
        scale_Modify_Coroutine = StartCoroutine(IScaleEffect(transform.localScale, Vector3.one, mouseOverTime));

        if (position_Modify_Coroutine != null)
            StopCoroutine(position_Modify_Coroutine);
        position_Modify_Coroutine = StartCoroutine(IMoveEffect(transform.position, originPos, mouseOverTime));
    }

    private IEnumerator IMoveEffect(Vector3 originPositioin, Vector3 targetPosition, float lerpTime)
    {
        //targetPosition = originPositioin + targetPosition;
        float elapsedTime = 0f;
        while (elapsedTime < lerpTime)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / lerpTime);

            transform.position = Vector3.Lerp(originPositioin, targetPosition, Mathf.Sin(t * Mathf.PI * 0.5f));
            yield return null;
        }
    }

    private IEnumerator IColorEffect(Color startColor, Color targetColor, float lerpTime)
    {
        Image[] cardImgs = GetComponentsInChildren<Image>();
        TextMeshProUGUI[] texts = GetComponentsInChildren<TextMeshProUGUI>();
        float elapsedTime = 0f;
        foreach (Image temp1 in cardImgs)
            temp1.color = startColor;
        foreach (TextMeshProUGUI temp2 in texts)
            temp2.color = startColor;

        while (elapsedTime < lerpTime)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / lerpTime);

            // cardImgs와 texts의 color의 알파값 조정
            foreach (Image img in cardImgs)
            {
                Color currentColor = Color.Lerp(startColor, targetColor, 1f - Mathf.Cos(t * Mathf.PI * 0.5f));
                img.color = currentColor;
            }

            foreach (TextMeshProUGUI text in texts)
            {
                Color currentColor = Color.Lerp(startColor, targetColor, 1f - Mathf.Cos(t * Mathf.PI * 0.5f));
                text.color = currentColor;
            }

            yield return null;
        }
        yield return null;

        Destroy(this.gameObject);
    }

    private IEnumerator IScaleEffect(Vector3 startScale, Vector3 targetScale, float lerpTime = 0.5f)
    {
        //뽑히는 카드의 스케일 조정
        //lerpTime에 걸쳐 transform.scale을 0에서 1로 변경
        float elapsedTime = 0f;
        transform.localScale = startScale;

        while (elapsedTime < lerpTime)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / lerpTime);
            Vector3 currentScale = Vector3.Lerp(startScale, targetScale, t);
            transform.localScale = currentScale;

            yield return null;
        }
        yield return null;
    }

    //1. 그래픽스 레이캐스팅 사용
    //2. 마우스오버 : 카드 확대(스케일 조정)
    //3. 드래그시작 : 카드 선택
    //4. 드래그중 : 마우스 위치까지 선으로 표시
    //5. 드래그종료 : 타일놓기/취소

    public void OnBeginDrag(PointerEventData eventData)
    {
        if(!Input.GetKey(KeyCode.Mouse0))
        {
            OnEndDrag(eventData);
            return;
        }
        //카드선택
        CallCard();
    }

    public void OnDrag(PointerEventData eventData) { }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (instancedObject == null || !instancedObject.gameObject.activeSelf)
            return;

        SetObjectOnMap();
    }

    private void CardSelectCheck()
    {

    }

    private bool CancelInput()
    {
        if(Input.GetKeyDown(KeyCode.Mouse1))
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

            GameManager.Instance.cardDeckController.cards.Remove(this.transform);
            GameManager.Instance.cardDeckController.SetCardPosition();

            float lerpTime = 0.4f;
            StartCoroutine(IColorEffect(Color.white, new Color(1,1,1,0),lerpTime));
            StartCoroutine(IMoveEffect(originPos, originPos + new Vector3(0, 150, 0), lerpTime));
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

    private void SetTile()
    {
        Tile tile = instancedObject.GetComponent<Tile>();
        if (tile != null)
        {
            NodeManager.Instance.emptyNodes.Remove(curNode);
            NodeManager.Instance.activeNodes.Add(curNode);
            tile.MoveTile(curNode);
        }
    }

    private void SetMonster()
    {
        Monster monster = instancedObject.GetComponent<Monster>();
        if (monster != null)
        {
            monster.SetStartPoint(curNode);
            monster.transform.position = curNode.transform.position;
            monster.Init();
        }
    }

    private void SetTrap()
    {
        Trap trap = instancedObject.GetComponent<Trap>();
        if (trap != null)
        {
            trap.transform.SetParent(curNode.transform);
            trap.Init();
        }
    }

    private void SetObjectOnMap(bool cancel = false)
    {
        bool discard = false;
        if(!cancel && curNode != null && curNode.setAvail)
        {
            switch (cardType)
            {
                case CardType.MapTile:
                    SetTile();
                    break;
                case CardType.Monster:
                    SetMonster();
                    break;
                case CardType.Trap:
                    SetTrap();
                    break;
            }
            discard = true;
        }

        Discard(discard);
        DrawLine(false);
        InputManager.Instance.settingCard = false;
        NodeManager.Instance.ResetAvail();
    }

    private void UpdateObjectPosition()
    {
        curNode = UtilHelper.RayCastTile();

        if(curNode != null && curNode.GuideActive)
        {
            instancedObject.transform.position = curNode.transform.position;
        }
        else
        {
            instancedObject.transform.position = new Vector3(0, 10000, 0);
        }
    }

    private void DrawLine(bool value = true)
    {
        //라인렌더러로 선표시
        Vector3 startPos = transform.position;

        GameManager.Instance.cardDeckController.DrawGuide(startPos, value);

    }

    private void UpdateObjectState()
    {
        UpdateObjectPosition();
        if(cardType == CardType.MapTile && Input.GetKeyDown(KeyCode.R))
        {
            Tile tile = instancedObject.GetComponent<Tile>();
            tile.RotateTile();
            ReadyForMapTile();
        }

        DrawLine();

        if (CancelInput())
            SetObjectOnMap(true);
    }

    private void ReadyForTrap()
    {
        NodeManager.Instance.ResetAvail();
        UtilHelper.SetAvail(true, NodeManager.Instance.activeNodes);
    }

    private void ReadyForMapTile()
    {
        NodeManager.Instance.ResetAvail();
        Tile tile = instancedObject.GetComponent<Tile>();
        NodeManager.Instance.SetTileAvail(tile);
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

    public void DrawEffect()
    {
        if (scale_Modify_Coroutine != null)
            StopCoroutine(scale_Modify_Coroutine);
        float lerpTime = 0.5f;
        scale_Modify_Coroutine = StartCoroutine(IScaleEffect(Vector3.zero, Vector3.one, lerpTime));
        Invoke("DrawEnd", lerpTime);
    }

    public void Init(CardType type, GameObject targetPrefab)
    {
        this.cardType = type;
        this.targetPrefab = targetPrefab;
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
