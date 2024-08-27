using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using UniRx;
using UniRx.Triggers;
using Cysharp.Threading.Tasks;
using System.Threading;
using Unity.VisualScripting;

public enum CardType
{
    None,
    MapTile,
    Monster,
    Trap,
    Environment,
    Spawner,
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

    protected virtual bool SetInput()
    {
        return Input.GetKeyUp(KeyCode.Mouse0);
    }

    private bool CancelInput()
    {
        return Input.GetKeyDown(KeyCode.Mouse1) || Input.GetKeyDown(KeyCode.F1) || Input.GetKeyDown(KeyCode.F2) || Input.GetKeyDown(KeyCode.F3) || Input.GetKeyDown(KeyCode.Escape) || GameManager.Instance.isPause;
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
            monster.Init();
            monster.SetStartPoint(curNode);
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

    protected virtual void SetObjectOnMap(bool cancel = false)
    {
        instancedObject.SetActive(!cancel && curNode != null && curNode.setAvail);
        bool recycle = GameManager.Instance.cardDeckController.IsRecycle;
        if (!cancel && recycle)
            GameManager.Instance.cardDeckController.AddCard(cardIndex);
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
            instancedObject = null;
        }
    }

    protected void EndCard()
    {
        DrawLine(false);
        curNode = null;
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
        Vector3 startPos = value ? transform.position : Vector3.zero;

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
                AudioManager.Instance.Play2DSound("Card_Tile_E", SettingManager.Instance._FxVolume);
                NodeManager.Instance.SetGuideState(GuideState.Tile, tile);
            }
            else if (Input.GetKeyDown(SettingManager.Instance.key_RotateLeft._CurKey))
            {
                Tile tile = instancedObject.GetComponent<Tile>();
                AudioManager.Instance.Play2DSound("Card_Tile_Q", SettingManager.Instance._FxVolume);
                tile.RotateTile(true);
                NodeManager.Instance.SetGuideState(GuideState.Tile, tile);
            }
        }

        if (CancelInput())
            SetObjectOnMap(true);

        DrawLine();
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

    public virtual void Init(Card targetCard)
    {
        this.cardIndex = targetCard.cardIndex;
        this.cardType = targetCard.cardType;
        targetPrefab = UtilHelper.GetCardPrefab(targetCard.cardType, targetCard.cardPrefabName);
        _cardInfo.Value = targetCard;
    }

    private CancellationTokenSource _tokenSource = new CancellationTokenSource();

    private async UniTaskVoid WaitForCard()
    {
        await UniTask.Yield(_tokenSource.Token);

        if (InputManager.Instance.settingCard) return;

        CallCard();
        while(!CancelInput() && !SetInput())
        {
            UpdateObjectState();
            await UniTask.Yield(_tokenSource.Token);
        }

        if (CancelInput())
            SetObjectOnMap(true);
        else
            SetObjectOnMap();
        
        EndCard();
        DrawLine(false);
    }

    protected virtual void Start()
    {
        Image image = GetComponent<Image>();
        var startStream = image.OnPointerDownAsObservable()
            .Subscribe(_ => WaitForCard().Forget());
    }

    private void OnEnable()
    {
        _tokenSource = new CancellationTokenSource();
    }

    private void OnDisable()
    {
        _tokenSource.Cancel();
        _tokenSource.Dispose();
        //EndCard();
    }
}
