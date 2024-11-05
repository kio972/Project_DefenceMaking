using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UniRx;
using Cysharp.Threading.Tasks;

public enum TileType
{
    Start,
    Path,
    Room,
    End,
    Door,
    Room_Single,
    Environment,
}

public interface ITileKind
{
    TileNode curNode { get; }
}

public interface IObjectKind
{
    Tile curTile { get; }
}

public class Tile : MonoBehaviour, ITileKind
{
    [SerializeField]
    private List<Direction> pathDirection = new List<Direction>();
    [SerializeField]
    private List<Direction> roomDirection = new List<Direction>();
    [SerializeField]
    private TileType tileType;

    private Animator tileAnimator;

    [SerializeField]
    private int roomMana;
    public ReactiveProperty<int> roomManaProperty { get; private set; } = new ReactiveProperty<int>();
    public int RoomMana
    {
        get
        {
            int curMana = roomMana;
            bool isRoom = tileType == TileType.Room || tileType == TileType.Room_Single || tileType == TileType.Door;
            if (isUpgraded && isRoom)
                curMana++;
            foreach (TileNode node in _curNode.neighborNodeDic.Values)
            {
                if (node.environment != null && node.environment is IManaSupply supply)
                    curMana += supply.manaValue;
            }

            roomManaProperty.Value = curMana;
            return curMana;
        }
    }

    private int supplyMana = 1;
    public int SupplyMana
    {
        get
        {
            int curMana = supplyMana;
            bool isPath = tileType == TileType.Path;
            if(isUpgraded && isPath)
                curMana++;
            foreach(TileNode node in _curNode.neighborNodeDic.Values)
            {
                if (node.environment != null && node.environment is IManaSupply supply)
                    curMana += supply.manaValue;
            }

            return curMana;
        }
    }

    private float _tileBaseSpeed = 1f;
    
    private Dictionary<UnitType, float> tileSpeedRateDic = new Dictionary<UnitType, float>()
    {
        { UnitType.Enemy, 1f }, { UnitType.Player, 1f }
    };

    public void UpdateTileSpeed(UnitType targetUnit)
    {
        foreach(var tile in _curNode.neighborNodeDic.Values)
        {
            if(tile.environment != null && tile.environment is ISpeedModify speedModify && speedModify.targetUnit == targetUnit)
            {

            }

        }
    }

    public TileType _TileType { get => tileType; }

    public List<Direction> PathDirection { get => pathDirection; }
    public List<Direction> RoomDirection { get => roomDirection; }

    private TileNode _curNode;

    public TileNode curNode { get => _curNode; }

    private bool removable = false;
    public bool IsRemovable
    {
        get
        {
            bool isRemovable = removable && !isDormant;
            foreach(var room in NodeManager.Instance.roomTiles)
            {
                if(room._IncludeRooms.Count > 1 && room._IncludeRooms.Contains(this))
                {
                    isRemovable = false;
                    break;
                }
            }

            return isRemovable;
        }

        set => removable = value;
    }

    public bool IsRemovableNow
    {
        get
        {
            bool remove = removable;

            if (remove)
                remove = !GameManager.Instance.IsAdventurererOnTile(_curNode);
            if (remove)
                remove = !GameManager.Instance.IsMonsterOnTile(_curNode);

            //if (UtilHelper.GetConnectedCount(this) >= 2)
            //    remove = false;

            return remove;
        }
    }

    private bool movable = false;

    private bool isDormant = false;

    public bool IsDormant { get => isDormant; }
    public bool Movable { get => movable; set { movable = value; } }
    public bool MovableNow
    {
        get
        {
            bool canMove = movable;
            if(canMove)
                canMove = !GameManager.Instance.IsAdventurererOnTile(_curNode);
            if (canMove)
                canMove = !GameManager.Instance.IsMonsterOnTile(_curNode);
            return canMove;
        }
    }

    public bool waitToMove = false;

    public Tile twin = null;

    public Trap trap = null;

    private MonsterSpawner spawner;
    public MonsterSpawner _Spanwer { get => spawner; }
    public bool HaveSpawner { get => spawner != null && !spawner.isEmpty; }

    public bool IsBigRoom = false;

    private Renderer tileRenderer;

    protected bool isTwin = false;
    
    private readonly Direction[] RoomRotations = { Direction.None, Direction.Left, Direction.LeftUp, Direction.RightUp, Direction.Right, Direction.RightDown, Direction.LeftDown };

    private bool _isUpgraded = false;
    public bool isUpgraded { get => _isUpgraded; set => _isUpgraded = value; }

    public int GetUnClosedCount()
    {
        // �ҿ��� ������ ������ Ÿ�� ������ ��ȯ�ϴ� �Լ�
        if (_curNode == null)
            return -1;

        int count = 0;
        count += UtilHelper.GetDirectionUnClosed(_curNode, PathDirection);
        count += UtilHelper.GetDirectionUnClosed(_curNode, RoomDirection, true);

        return count;
    }

    public void MoveTile(TileNode nextNode, bool isActive = true)
    {
        //_curNode?.SetFog(true);
        if(_curNode != null)
        {
            NodeManager.Instance.RemoveSightNode(_curNode);
            _curNode.curTile = null;
            NodeManager.Instance.SetActiveNode(_curNode, false);
        }

        transform.SetParent(nextNode.transform, false);

        _curNode = nextNode;
        NodeManager.Instance.AddSightNode(_curNode);
        //_curNode?.SetFog(false);
        if (isActive && !NodeManager.Instance._ActiveNodes.Contains(nextNode))
            NodeManager.Instance.SetActiveNode(nextNode, true);

        nextNode.curTile = this;
        transform.position = nextNode.transform.position;
        NodeManager.Instance.ExpandEmptyNode(nextNode, 20);

        if (tileType == TileType.End)
        {
            NodeManager.Instance.endPoint = nextNode;
            if(GameManager.Instance.king != null)
                GameManager.Instance.king.SetTile(nextNode);
        }

        NodeManager.Instance.DormantTileCheck();
        GameManager.Instance.UpdateTotalMana();

        //if(GameManager.Instance.IsInit && !GameManager.Instance.speedController.Is_Game_Continuable())
        //    GameManager.Instance.speedController.SetSpeedZero();

        if (isActive)
        {
            if(tileType == TileType.Room_Single || tileType == TileType.Room || tileType == TileType.Door)
                NodeManager.Instance.RoomCheck(this);
        }

        NodeManager.Instance.UpdateMinMaxRowCol(_curNode.row, _curNode.col);
        //NodeManager.Instance.UpdateSightNode();
        GameManager.Instance.CheckBattlerCollapsed();

        if(GameManager.Instance.IsInit)
            AudioManager.Instance.Play2DSound("FistHitDoor_ZA01.261", SettingManager.Instance._FxVolume);
    }

    private void SetTileVisible(bool value)
    {
        if (tileRenderer == null)
            tileRenderer = GetComponentInChildren<Renderer>();
        if (tileRenderer == null) return;

        tileRenderer.enabled = value;
    }

    private void UpdateMoveTilePos(TileNode _curNode)
    {
        if (_curNode != null && NodeManager.Instance.virtualNodes.Contains(_curNode) || _curNode == this._curNode)
            twin.transform.position = _curNode.transform.position + new Vector3(0, 0.01f, 0);
        else
            twin.transform.position = new Vector3(0, 10000, 0);

        SetTileVisible(_curNode != this._curNode);
    }

    public void EndMoveing(bool resetNode = true)
    {
        twin.gameObject.SetActive(false);
        waitToMove = false;
        InputManager.Instance.settingCard = false;
        InputManager.Instance.ResetTileClick();
        if (resetNode)
            NodeManager.Instance.SetActiveNode(this._curNode, true);
        NodeManager.Instance.SetGuideState(GuideState.None);
        SetTileVisible(true);
    }

    public void EndMove(TileNode _curNode)
    {
        bool resetNode = true;
        if (_curNode != null && _curNode.setAvail)
        {
            transform.rotation = twin.transform.rotation;
            rotationCount.Value = twin.rotationCount.Value;
            pathDirection = new List<Direction>(twin.pathDirection);
            roomDirection = new List<Direction>(twin.roomDirection);

            NodeManager.Instance.SetActiveNode(this._curNode, false);
            NodeManager.Instance.SetActiveNode(_curNode, true);
            MoveTile(_curNode);

            //AudioManager.Instance.Play2DSound("Click_tile", SettingManager.Instance._FxVolume);
            resetNode = false;

            //if (SettingManager.Instance.autoPlay == AutoPlaySetting.setTile || SettingManager.Instance.autoPlay == AutoPlaySetting.always)
            //    GameManager.Instance.speedController.SetSpeedPrev(false);
        }

        EndMoveing(resetNode);
    }

    public ReactiveProperty<int> rotationCount { get; private set; } = new ReactiveProperty<int>(0);
    private void RotateDirection(bool reverse = false)
    {
        float rate = 60f;
        if (reverse)
            rate *= -1f;
        int nextvalue = rotationCount.Value + (reverse ? -1 : 1);
        if (nextvalue < 0)
            nextvalue += 6;
        nextvalue %= 6;
        rotationCount.Value = nextvalue;
        transform.rotation *= Quaternion.Euler(0f, rate, 0f);
    }


    public List<Direction> RotateDirection(List<Direction> pathDirection, bool reverse = false)
    {
        //���� ����� pathDirection�� roomDirection�� 60�� ȸ���ϴ� �Լ�
        List<Direction> newDirection = new List<Direction>();
        foreach (Direction dir in pathDirection)
        {
            if (dir == Direction.None) continue;
            int dirNum = 1;
            if (reverse) dirNum = -1;

            int nextDir = (int)dir + dirNum;
            if (nextDir >= RoomRotations.Length) nextDir = 1;
            else if (nextDir <= 0) nextDir = RoomRotations.Length - 1;

            newDirection.Add(RoomRotations[nextDir]);
        }

        return newDirection;
    }

    public void RotateTile(bool reverse = false)
    {
        pathDirection = RotateDirection(pathDirection, reverse);
        roomDirection = RotateDirection(roomDirection, reverse);
        RotateDirection(reverse);
        NodeManager.Instance.SetGuideState(GuideState.Tile, this);
    }

    public void ResetTwin()
    {
        twin.pathDirection = pathDirection;
        twin.roomDirection = roomDirection;
    }

    private void SetTwin()
    {
        if (twin == null)
        {
            twin = Instantiate(this, transform);
            Collider[] colliders = twin.GetComponentsInChildren<Collider>();
            foreach (Collider collider in colliders)
                collider.enabled = false;
            twin.isTwin = true;
            twin.waitToMove = false;
        }
        twin.gameObject.SetActive(true);
        twin.pathDirection = new List<Direction>(pathDirection);
        twin.roomDirection = new List<Direction>(roomDirection);
        twin.transform.rotation = transform.rotation;
        twin.rotationCount.Value = rotationCount.Value;
        //NodeManager.Instance.SetActiveNode(this._curNode, false);
    }

    public async UniTaskVoid ReadyForMove()
    {
        SetTwin();
        switch (tileType)
        {
            case TileType.End:
                NodeManager.Instance.SetGuideState(GuideState.Tile, twin);
                break;
            default:
                NodeManager.Instance.SetGuideState(GuideState.Tile, twin);
                break;
        }

        //_curNode.SetAvail(true);
        TileMoveCheck();
        await UniTask.WaitUntil(() => !Input.GetKey(SettingManager.Instance.key_BasicControl._CurKey) && !Input.GetKeyUp(SettingManager.Instance.key_BasicControl._CurKey));
        //AudioManager.Instance.Play2DSound("Click_tile", SettingManager.Instance._FxVolume);
        waitToMove = true;
    }

    public TileNode TileMoveCheck()
    {
        TileNode _curNode = UtilHelper.RayCastTile();
        UpdateMoveTilePos(_curNode);
        return _curNode;
    }

    private void MonsterOutCheck()
    {
        //if (monster == null)
        //    return;

        //if ((monster.transform.position - transform.position).magnitude > 0.5f)
        //{
        //    monster = null;
        //    if (NodeManager.Instance._GuideState == GuideState.Monster)
        //        NodeManager.Instance.SetGuideState(GuideState.Monster);
        //}
    }

    private bool DormantAwake()
    {
        //����� Ÿ�� �� active������ ��尡 ������ return true
        foreach(Direction direction in PathDirection)
        {
            TileNode target = _curNode.neighborNodeDic[direction];
            if (target == null || target.curTile == null)
                continue;
            if (UtilHelper.IsTileConnected(_curNode, direction, target.curTile.PathDirection))
                return true;
            if (UtilHelper.IsTileConnected(_curNode, direction, target.curTile.PathDirection))
                return true;
        }

        return false;
    }

    public void DormantAwakeCheck()
    {
        if(DormantAwake())
        {
            isDormant = false;
            if(!GameManager.Instance.speedController.Is_Tile_Connected(this._curNode))
            {
                isDormant = true;
                return;
            }

            movable = false;
            

            // ����ȹ��
            NodeManager.Instance.dormantTile.Remove(this);
            //GameManager.Instance.gold += 100;
            //IRewardObject rewardObject = GetComponentInChildren<IRewardObject>();
            //rewardObject?.GetReward();
        }
    }

    public void AddSpawner(MonsterSpawner spawner)
    {
        this.spawner = spawner;
    }

    public void RemoveSpawner()
    {
        spawner.Dead();
        spawner = null;
    }

    public void RemoveTile()
    {
        NodeManager.Instance.SetActiveNode(_curNode, false);
        InputManager.Instance.ResetTileClick();
        tileAnimator.SetTrigger("Destroy");

        AudioManager.Instance.Play2DSound("FistHitDoor_ZA01.262", SettingManager.Instance._FxVolume);

        GameManager.Instance.gold += PassiveManager.Instance._TileDesturctIncome;
        trap?.DestroyTrap();
        Destroy(this.gameObject, 1.0f);

        if (spawner != null)
            RemoveSpawner();
        _curNode.curTile = null;

        NodeManager.Instance.RemoveTile(this);
        GameManager.Instance.CheckBattlerCollapsed();
        GameManager.Instance.UpdateTotalMana();
    }

    public TileData GetTileData()
    {
        TileData tile = new TileData();
        tile.id = name.ToString().Replace("(Clone)", "");
        tile.row = _curNode.row;
        tile.col = _curNode.col;
        tile.rotation = rotationCount.Value;
        tile.isDormant = isDormant;
        tile.isRemovable = removable;
        tile.trapId = trap != null ? trap.BattlerID : "";
        tile.trapDuration = trap != null ? trap.Duration : 0;

        return tile;
    }

    public void Init(TileNode targetNode, bool dormant = false, bool removable = true, bool playAnim = true)
    {
        if(tileAnimator == null)
            tileAnimator = GetComponentInChildren<Animator>();
        NodeManager.Instance.SetActiveNode(targetNode, true);
        isDormant = dormant;
        MoveTile(targetNode);
        movable = tileType == TileType.End ? true : false;
        this.removable = removable;
        if(isDormant)
        {
            NodeManager.Instance.dormantTile.Add(this);
            //GameObject goldBox = Resources.Load<GameObject>("Prefab/Objects/GoldBox");
            //goldBox = Instantiate(goldBox, transform, true);
            //goldBox.transform.position = transform.position;
        }
        if(playAnim)
            tileAnimator?.SetTrigger("Set");

        NodeManager.Instance.SetTile(this);
    }

    protected virtual void Update()
    {
        if (isTwin)
            return;

        //MonsterOutCheck();

        if(waitToMove)
        {
            TileNode curTile = TileMoveCheck();
            if (Input.GetKeyDown(SettingManager.Instance.key_RotateRight._CurKey))
            {
                AudioManager.Instance.Play2DSound("Card_Tile_E", SettingManager.Instance._FxVolume);
                twin.RotateTile();
            }
            else if(Input.GetKeyDown(SettingManager.Instance.key_RotateLeft._CurKey))
            {
                AudioManager.Instance.Play2DSound("Card_Tile_Q", SettingManager.Instance._FxVolume);
                twin.RotateTile(true);
            }

            if (!MovableNow || Input.GetKeyUp(SettingManager.Instance.key_CancelControl._CurKey) || Input.GetKeyDown(KeyCode.Escape))
            {
                EndMoveing();
            }
            else if (Input.GetKeyUp(SettingManager.Instance.key_BasicControl._CurKey) && curTile != null)
            {
                EndMove(curTile);
            }
        }
    }
}
