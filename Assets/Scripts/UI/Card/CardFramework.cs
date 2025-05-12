using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using UniRx.Triggers;
using Cysharp.Threading.Tasks;
using System.Threading;

public enum CardType
{
    None,
    PathTile,
    Monster,
    Trap,
    Environment,
    Spawner,
    ObjectForPath,
    Magic,
    RoomTile,
}

public enum CardGrade
{
    none,
    normal,
    rare,
    epic,
    legend,
}

public class CardFramework : MonoBehaviour
{
    [SerializeField]
    private CardType cardType;
    [SerializeField]
    private GameObject targetPrefab;

    protected GameObject instancedObject;
    private TileNode _curNode = null;

    protected int cardIndex = -1;

    public ReactiveProperty<Card> _cardInfo { get; private set; } = new ReactiveProperty<Card>();

    protected CompositeDisposable disposables = new CompositeDisposable();

    bool isUsingCard = false;

    public int handIndex;

    private void SetTile(TileNode curNode)
    {
        Tile tile = instancedObject.GetComponent<Tile>();
        if (tile != null)
        {
            tile.Init(curNode);

            //if (SettingManager.Instance.autoPlay == AutoPlaySetting.setTile || SettingManager.Instance.autoPlay == AutoPlaySetting.always)
            //    GameManager.Instance.speedController.SetSpeedPrev(false);
        }

        //AudioManager.Instance.Play2DSound("Click_tile_01", SettingManager.Instance._FxVolume);
    }

    private void SetEnvironment(TileNode curNode)
    {
        Environment environment = instancedObject.GetComponent<Environment>();
        if (environment != null)
        {
            environment.Init(curNode);

            //if (SettingManager.Instance.autoPlay == AutoPlaySetting.always)
            //    GameManager.Instance.speedController.SetSpeedPrev(false);
        }

        //if (GameManager.Instance.IsInit)
        //    AudioManager.Instance.Play2DSound("FistHitDoor_ZA01.261", SettingManager.Instance._FxVolume);
    }

    private void SetMonster(TileNode curNode)
    {
        Monster monster = instancedObject.GetComponent<Monster>();
        if (monster != null)
        {
            monster.SetStartPoint(curNode);
            monster.Init();
            //curNode.curTile.monster = monster;

            if (SettingManager.Instance.autoPlay == AutoPlaySetting.always)
                GameManager.Instance.speedController.SetSpeedPrev(false);
        }

        //AudioManager.Instance.Play2DSound("Set_monster", SettingManager.Instance._FxVolume);
    }

    private void SetObejct(TileNode curNode)
    {
        IObjectKind obj = instancedObject.GetComponent<IObjectKind>();
        obj.SetTileInfo(curNode.curTile);
        if(obj is Battler battler)
        {
            battler.SetStartPoint(curNode);
            battler.Init();
        }
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

        //AudioManager.Instance.Play2DSound("Set_trap", SettingManager.Instance._FxVolume);
    }

    protected virtual void SetObjectOnMap(bool cancel = false)
    {
        if(cancel)
        {
            if(instancedObject != null)
                instancedObject.SetActive(false);
            return;
        }

        if(cardType == CardType.Magic)
        {
            bool isActive = Input.mousePosition.y > 300;
            if (isActive)
            {
                SpecialCardEffect effect = gameObject.AddComponent<SpecialCardEffect>();
                effect.SetCard(this);
                effect.SendMessage(_cardInfo.Value.cardPrefabName, SendMessageOptions.DontRequireReceiver);
            }
            return;
        }

        instancedObject.SetActive(_curNode != null && _curNode.setAvail);
        if (_curNode != null && _curNode.setAvail)
        {
            switch (cardType)
            {
                case CardType.PathTile:
                    SetTile(_curNode);
                    break;
                case CardType.RoomTile:
                    SetTile(_curNode);
                    break;
                case CardType.Monster:
                    SetMonster(_curNode);
                    break;
                case CardType.Trap:
                    SetTrap(_curNode);
                    break;
                case CardType.Environment:
                    SetEnvironment(_curNode);
                    break;
                case CardType.ObjectForPath:
                    SetObejct(_curNode);
                    break;
            }
            instancedObject.transform.position = _curNode.transform.position;
            instancedObject = null;
        }
    }

    public void EndCard(bool isCancel)
    {
        SetObjectOnMap(isCancel);

        DrawLine(false);
        _curNode = null;
        InputManager.Instance.settingCard = false;
        NodeManager.Instance.SetGuideState(GuideState.None);
        isUsingCard = false;

        GameManager.Instance.cardDeckController.curHandlingObject.Value = null;
        GameManager.Instance._InGameUI.tileArrowUI?.SetOFF();
        RoomManaPool.Instance.ShowManaGuide(false, null);
        RoomManaPool.Instance.HideAllManaText();
    }

    private void UpdateObjectPosition()
    {
        TileNode curNode = UtilHelper.RayCastTile();
        if (curNode == _curNode)
            return;

        _curNode = curNode;

        if(_curNode != null && _curNode.GuideActive)
        {
            instancedObject.transform.SetParent(_curNode.transform, false);
            instancedObject.transform.position = _curNode.transform.position;
            if(_curNode.setAvail)
            {
                Tile tile = instancedObject.GetComponent<Tile>();
                if(tile != null)
                {
                    tile.AutoRotate(curNode);
                    //AudioManager.Instance.Play2DSound("Card_Tile_E", SettingManager.Instance._FxVolume);
                }
            }
        }
        else
        {
            instancedObject.transform.position = new Vector3(0, 10000, 0);
        }

        RoomManaPool.Instance.HideAllManaText();
        DrawArrowUI(curNode);
        DrawManaUI(curNode);
        DrawRoomGuide(curNode, instancedObject);
    }

    private void DrawRoomGuide(TileNode curNode, GameObject instancedObject)
    {
        Tile tile = instancedObject.GetComponent<Tile>();
        if (tile != null && curNode != null && _curNode.GuideActive && _curNode.setAvail && tile._TileType is TileType.Room or TileType.Door or TileType.Room_Single)
        {
            tile.curNode = _curNode;
            _curNode.tileKind = tile;
            bool isCompleteRoom = false;
            var tiles = NodeManager.Instance.BFSRoom(tile, out isCompleteRoom);
            RoomManaPool.Instance.ShowManaGuide(true, tiles, isCompleteRoom);
            tile.curNode = null;
        }
        else
            RoomManaPool.Instance.ShowManaGuide(false, null);
    }

    private void DrawArrowUI(TileNode curNode)
    {
        ITileKind tileKind = instancedObject.GetComponent<ITileKind>();
        if (curNode != null && _curNode.GuideActive && tileKind != null)
        {
            GameManager.Instance._InGameUI.tileArrowUI?.SetArrow(tileKind, curNode);
        }
        else
            GameManager.Instance._InGameUI.tileArrowUI?.SetOFF();
    }

    private void DrawManaUI(TileNode curNode)
    {
        ITileManaEffect manaEffect = instancedObject.GetComponent<ITileManaEffect>();
        if (manaEffect != null && curNode != null && curNode.GuideActive)
        {
            foreach(var node in curNode.neighborNodeDic.Values)
            {
                if (node.tileKind == null)
                    continue;
                string text = manaEffect.GetManaText(node.tileKind);
                if (text == null)
                    continue;
                RoomManaPool.Instance.ShowManaText(node.transform, text);
            }
        }
    }

    private void DrawLine(bool value = true)
    {
        //라인렌더러로 선표시
        Vector3 startPos = value ? transform.position : Vector3.zero;

        GameManager.Instance.cardDeckController.DrawGuide(startPos, value);

    }

    private void UpdateObjectState()
    {
        if (cardType == CardType.Magic)
            return;

        UpdateObjectPosition();
        if (cardType is CardType.PathTile or CardType.RoomTile)
            RotateCheck();

        DrawLine();
    }

    private void RotateCheck()
    {
        if (GameManager.Instance.rotateLock)
            return;

        if (!Input.GetKeyDown(SettingManager.Instance.key_RotateRight._CurKey) && !Input.GetKeyDown(SettingManager.Instance.key_RotateLeft._CurKey))
            return;

        Tile tile = instancedObject.GetComponent<Tile>();
        if (tile == null)
            return;

        bool isReverse = Input.GetKeyDown(SettingManager.Instance.key_RotateLeft._CurKey);

        if (_curNode.setAvail)
            tile.RotateToNext(_curNode, isReverse);
        else
            tile.RotateTile(isReverse);

        AudioManager.Instance.Play2DSound(isReverse ? "Card_Tile_Q" : "Card_Tile_E", SettingManager.Instance._FxVolume);
    }

    private void InstanceObject()
    {
        if (instancedObject == null && targetPrefab != null)
            instancedObject = Instantiate(targetPrefab);
        else
            instancedObject.SetActive(true);

        if (cardType == CardType.Monster)
        {
            Monster monster = instancedObject.GetComponent<Monster>();
            monster.SetRotation();
        }

        instancedObject.transform.position = new Vector3(0, 10000, 0);

        GameManager.Instance.cardDeckController.curHandlingObject.Value = instancedObject;
    }

    public void CallCard()
    {
        if (isUsingCard)
            return;

        isUsingCard = true;
        InputManager.Instance.settingCard = true;

        if (cardType != CardType.Magic)
            InstanceObject(); //targetPrefab생성

        //카드 종류별 처리
        switch (cardType)
        {
            case CardType.PathTile:
                Tile tile = instancedObject.GetComponent<Tile>();
                NodeManager.Instance.SetGuideState(GuideState.Tile, tile);
                break;
            case CardType.RoomTile:
                Tile room = instancedObject.GetComponent<Tile>();
                NodeManager.Instance.SetGuideState(GuideState.Tile, room);
                break;
            case CardType.Monster:
                NodeManager.Instance.SetGuideState(GuideState.Monster);
                break;
            case CardType.Trap:
                NodeManager.Instance.SetGuideState(GuideState.ObjectForPath);
                break;
            case CardType.Environment:
                NodeManager.Instance.SetGuideState(GuideState.Environment);
                break;
            case CardType.ObjectForPath:
                NodeManager.Instance.SetGuideState(GuideState.ObjectForPath);
                break;
            case CardType.Magic:
                NodeManager.Instance.SetGuideState(GuideState.None);
                //Image image = transform.GetComponent<Image>();
                //image.raycastTarget = false;
                CardUIEffect effect = GetComponent<CardUIEffect>();
                effect.OnMagicDrag().Forget();
                effect.MagicDrag().Forget();
                break;
        }

        UpdateObjectState();
    }

    public virtual void Init(Card targetCard)
    {
        this.cardIndex = targetCard.cardIndex;
        this.cardType = targetCard.cardType;
        targetPrefab = UtilHelper.GetCardPrefab(targetCard.cardType, targetCard.cardPrefabName);
        _cardInfo.Value = targetCard;
    }

    private void Update()
    {
        if (!isUsingCard)
            return;

        UpdateObjectState();
    }
}
