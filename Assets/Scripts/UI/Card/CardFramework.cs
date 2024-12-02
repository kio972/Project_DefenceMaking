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
    private TileNode curNode = null;

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

        if (GameManager.Instance.IsInit)
            AudioManager.Instance.Play2DSound("FistHitDoor_ZA01.261", SettingManager.Instance._FxVolume);
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

        AudioManager.Instance.Play2DSound("Set_monster", SettingManager.Instance._FxVolume);
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

        AudioManager.Instance.Play2DSound("Set_trap", SettingManager.Instance._FxVolume);
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

        instancedObject.SetActive(curNode != null && curNode.setAvail);
        if (curNode != null && curNode.setAvail)
        {
            switch (cardType)
            {
                case CardType.PathTile:
                    SetTile(curNode);
                    break;
                case CardType.RoomTile:
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
                case CardType.ObjectForPath:
                    SetObejct(curNode);
                    break;
            }
            instancedObject.transform.position = curNode.transform.position;
            instancedObject = null;
        }
    }

    public void EndCard(bool isCancel)
    {
        SetObjectOnMap(isCancel);

        DrawLine(false);
        curNode = null;
        InputManager.Instance.settingCard = false;
        NodeManager.Instance.SetGuideState(GuideState.None);
        isUsingCard = false;
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
        Vector3 startPos = value ? transform.position : Vector3.zero;

        GameManager.Instance.cardDeckController.DrawGuide(startPos, value);

    }

    private void UpdateObjectState()
    {
        if (cardType == CardType.Magic)
            return;

        UpdateObjectPosition();
        if(cardType is CardType.PathTile or CardType.RoomTile)
        {
            if (Input.GetKeyDown(SettingManager.Instance.key_RotateRight._CurKey) &&!GameManager.Instance.rotateLock)
            {
                Tile tile = instancedObject.GetComponent<Tile>();
                tile.RotateTile();
                AudioManager.Instance.Play2DSound("Card_Tile_E", SettingManager.Instance._FxVolume);
                NodeManager.Instance.SetGuideState(GuideState.Tile, tile);
            }
            else if (Input.GetKeyDown(SettingManager.Instance.key_RotateLeft._CurKey) && !GameManager.Instance.rotateLock)
            {
                Tile tile = instancedObject.GetComponent<Tile>();
                AudioManager.Instance.Play2DSound("Card_Tile_Q", SettingManager.Instance._FxVolume);
                tile.RotateTile(true);
                NodeManager.Instance.SetGuideState(GuideState.Tile, tile);
            }
        }

        DrawLine();
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
