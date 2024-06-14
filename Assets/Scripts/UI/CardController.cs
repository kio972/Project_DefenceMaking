using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class CardController : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField]
    private Image card_Frame;
    [SerializeField]
    private Image card_Frame2;
    [SerializeField]
    private Image card_Frame_Mask;
    [SerializeField]
    private Image card_illust;
    [SerializeField]
    private Image card_Rank;
    [SerializeField]
    private LanguageText card_Name;
    [SerializeField]
    private LanguageText card_Description;
    [SerializeField]
    private CardType cardType;
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
    public Quaternion originRot;

    public int originSiblingIndex = -1;

    [SerializeField]
    private float mouseOverTime = 0.2f;

    private int cardIndex = -1;

    private void DrawEnd()
    {
        drawEnd = true;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (GameManager.Instance.cardLock)
            return;

        if (!drawEnd)
            return;

        if (scale_Modify_Coroutine != null)
            StopCoroutine(scale_Modify_Coroutine);
        scale_Modify_Coroutine = StartCoroutine(UtilHelper.IScaleEffect(card_Frame.transform, card_Frame.transform.localScale, Vector3.one * 1.5f, mouseOverTime));

        if (position_Modify_Coroutine != null)
            StopCoroutine(position_Modify_Coroutine);
        position_Modify_Coroutine = StartCoroutine(UtilHelper.IMoveEffect(card_Frame.transform, card_Frame.transform.position, new Vector3(originPos.x, 180, originPos.z), mouseOverTime));

        card_Frame.transform.rotation = Quaternion.identity;
        transform.SetAsLastSibling();

        AudioManager.Instance.Play2DSound("Click_card", SettingManager.Instance._FxVolume);
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        if (!drawEnd)
            return;

        if (scale_Modify_Coroutine != null)
            StopCoroutine(scale_Modify_Coroutine);
        scale_Modify_Coroutine = StartCoroutine(UtilHelper.IScaleEffect(card_Frame.transform, card_Frame.transform.localScale, Vector3.one, mouseOverTime));

        if (position_Modify_Coroutine != null)
            StopCoroutine(position_Modify_Coroutine);
        position_Modify_Coroutine = StartCoroutine(UtilHelper.IMoveEffect(card_Frame.transform, card_Frame.transform.position, originPos, mouseOverTime));

        card_Frame.transform.rotation = originRot;
        if (originSiblingIndex != -1)
            transform.SetSiblingIndex(originSiblingIndex);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (GameManager.Instance.cardLock)
            return;

        InputManager.Instance.ResetTileClick();

        if (!Input.GetKey(KeyCode.Mouse0))
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

    private bool CancelInput()
    {
        if(Input.GetKeyDown(KeyCode.Mouse1))
        {
            return true;
        }

        return false;
    }

    private void Discard(bool value, bool isRecycle)
    {
        if(value)
        {
            //타일 놓기
            instancedObject = null;
            GameManager.Instance.cardDeckController.hand_CardNumber--;

            GameManager.Instance.cardDeckController.DiscardCard(this.transform, cardIndex);
            //GameManager.Instance.cardDeckController.cards.Remove(this.transform);
            GameManager.Instance.cardDeckController.SetCardPosition();

            float lerpTime = 0.4f;
            StartCoroutine(UtilHelper.IColorEffect(transform, Color.white, new Color(1, 1, 1, 0), lerpTime, () => { Destroy(this.gameObject); }));
            if (isRecycle)
            {
                StartCoroutine(UtilHelper.IMoveEffect(transform, originPos, GameManager.Instance.cardDeckController.transform.position, lerpTime));
                StartCoroutine(UtilHelper.IScaleEffect(transform, Vector3.one, Vector3.zero, lerpTime));
            }
            else
                StartCoroutine(UtilHelper.IMoveEffect(transform, originPos, originPos + new Vector3(0, 150, 0), lerpTime));
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

    private void SetTile(TileNode curNode)
    {
        Tile tile = instancedObject.GetComponent<Tile>();
        if (tile != null)
        {
            tile.Init(curNode);

            //if (SettingManager.Instance.autoPlay == AutoPlaySetting.setTile || SettingManager.Instance.autoPlay == AutoPlaySetting.always)
            //    GameManager.Instance.speedController.SetSpeedPrev(false);
        }

        AudioManager.Instance.Play2DSound("Click_tile", SettingManager.Instance._FxVolume);
    }

    private void SetEnvironment(TileNode curNode)
    {
        Environment environment = instancedObject.GetComponent<Environment>();
        if (environment != null)
        {
            NodeManager.Instance.emptyNodes.Remove(curNode);
            NodeManager.Instance.SetActiveNode(curNode, true);
            environment.Init(curNode);

            if (SettingManager.Instance.autoPlay == AutoPlaySetting.always)
                GameManager.Instance.speedController.SetSpeedPrev(false);
        }

        AudioManager.Instance.Play2DSound("Click_tile", SettingManager.Instance._FxVolume);
    }

    private void SetMonster(TileNode curNode)
    {
        Monster monster = instancedObject.GetComponent<Monster>();
        if (monster != null)
        {
            monster.SetStartPoint(curNode);
            monster.transform.position = curNode.transform.position;
            monster.Init();
            //curNode.curTile.monster = monster;

            if (SettingManager.Instance.autoPlay == AutoPlaySetting.always)
                GameManager.Instance.speedController.SetSpeedPrev(false);
        }

        AudioManager.Instance.Play2DSound("Set_monster", SettingManager.Instance._FxVolume);
    }

    private void SetTrap(TileNode curNode)
    {
        Trap trap = instancedObject.GetComponent<Trap>();
        if (trap != null)
        {
            trap.transform.SetParent(curNode.curTile.transform);
            trap.Init(curNode.curTile);

            if (SettingManager.Instance.autoPlay == AutoPlaySetting.always)
                GameManager.Instance.speedController.SetSpeedPrev(false);
        }

        AudioManager.Instance.Play2DSound("Set_trap", SettingManager.Instance._FxVolume);
    }

    private void SetObjectOnMap(bool cancel = false)
    {
        bool discard = false;
        bool recycle = GameManager.Instance.cardDeckController.IsRecycle;
        if (!cancel && recycle)
        {
            GameManager.Instance.cardDeckController.AddCard(cardIndex);
            discard = true;
        }
        else if (!cancel && curNode != null && curNode.setAvail)
        {
            switch (cardType)
            {
                case CardType.MapTile:
                    SetTile(curNode);
                    break;
                case CardType.Monster:
                    SetMonster(curNode);
                    break;
                case CardType.Trap:
                    SetTrap(curNode);
                    break;
                case CardType.Environment:
                    SetEnvironment(curNode);
                    break;
            }
            discard = true;
        }
        
        Discard(discard, recycle);
        DrawLine(false);
        InputManager.Instance.settingCard = false;
        NodeManager.Instance.SetGuideState(GuideState.None);
    }

    private void UpdateObjectPosition()
    {
        curNode = UtilHelper.RayCastTile();

        if(curNode != null && curNode.GuideActive)
        {
            instancedObject.transform.SetParent(curNode.transform, false);
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
        if(cardType == CardType.MapTile)
        {
            if (Input.GetKeyDown(SettingManager.Instance.key_RotateRight._CurKey))
            {
                Tile tile = instancedObject.GetComponent<Tile>();
                tile.RotateTile();
                NodeManager.Instance.SetGuideState(GuideState.Tile, tile);
            }
            else if (Input.GetKeyDown(SettingManager.Instance.key_RotateLeft._CurKey))
            {
                Tile tile = instancedObject.GetComponent<Tile>();
                tile.RotateTile(true);
                NodeManager.Instance.SetGuideState(GuideState.Tile, tile);
            }
        }

        DrawLine();

        if (CancelInput())
            SetObjectOnMap(true);
    }


    public void CallCard()
    {
        //targetPrefab생성
        if(instancedObject == null)
            instancedObject = Instantiate(targetPrefab);
        else
            instancedObject.SetActive(true);

        if(cardType == CardType.Monster)
        {
            Monster monster = instancedObject.GetComponent<Monster>();
            monster.SetRotation();
        }

        InputManager.Instance.settingCard = true;
        //카드 종류별 처리
        switch (cardType)
        {
            case CardType.MapTile:
                Tile tile = instancedObject.GetComponent<Tile>();
                NodeManager.Instance.SetGuideState(GuideState.Tile, tile);
                break;
            case CardType.Monster:
                NodeManager.Instance.SetGuideState(GuideState.Monster);
                break;
            case CardType.Trap:
                NodeManager.Instance.SetGuideState(GuideState.Trap);
                break;
            case CardType.Environment:
                NodeManager.Instance.SetGuideState(GuideState.Environment);
                break;
        }
    }

    public void DrawEffect()
    {
        if (scale_Modify_Coroutine != null)
            StopCoroutine(scale_Modify_Coroutine);
        float lerpTime = 0.5f;
        scale_Modify_Coroutine = StartCoroutine(UtilHelper.IScaleEffect(transform, Vector3.zero, Vector3.one, lerpTime));
        Invoke("DrawEnd", lerpTime);
    }

    private string GetFrameName(string cardFrame)
    {
        switch(cardFrame)
        {
            case "road":
                return "cardFrame_04";
            case "room":
                return "cardFrame_05";
            case "roomPart":
                return "cardFrame_05";
            case "environment":
                return "cardFrame_03";
        }
        return null;
    }

    private void SetCardUI(Card targetCard)
    {
        Sprite frame1 = SpriteList.Instance.LoadSprite(GetFrameName(targetCard.cardFrame));
        card_Frame.sprite = frame1;
        //Sprite frame2 = SpriteList.Instance.LoadSprite("cardFrame2_" + targetCard.cardFrame);
        //card_Frame2.sprite = frame2;
        //card_Frame_Mask.sprite = frame2;

        Sprite cardRank = SpriteList.Instance.LoadSprite("cardRank_" + targetCard.cardGrade.ToString());
        card_Rank.sprite = cardRank;
        card_Rank.gameObject.SetActive(cardType != CardType.MapTile && cardType != CardType.Environment);

        card_Name.ChangeLangauge(SettingManager.Instance.language, targetCard.cardName);
        card_Description.ChangeLangauge(SettingManager.Instance.language, targetCard.cardDescription);

        Sprite illur = SpriteList.Instance.LoadSprite(targetCard.cardPrefabName);
        card_illust.sprite = illur;
    }

    public void Init(Card targetCard)
    {
        this.cardIndex = targetCard.cardIndex;
        this.cardType = targetCard.cardType;
        targetPrefab = UtilHelper.GetCardPrefab(targetCard.cardType, targetCard.cardPrefabName);
        SetCardUI(targetCard);
    }

    // Update is called once per frame
    void Update()
    {
        if (instancedObject != null && instancedObject.gameObject.activeSelf)
            UpdateObjectState();
    }
}
