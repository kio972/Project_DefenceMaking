using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UniRx;
using Cysharp.Threading.Tasks;
using System.Linq;
using System.Text;
using FMODUnity;

public enum TileType
{
    Start,
    Path,
    Room,
    End,
    Door,
    Room_Single,
    Environment,
    Herb,
    Special,
}

public interface ITileKind
{
    TileNode curNode { get; }
}

public interface IObjectKind
{
    Tile curTile { get; }
    void SetTileInfo(Tile tile);
}

public interface IDestructableObjectKind : IObjectKind
{
    void DestroyObject();
}

public class Tile : MonoBehaviour, ITileKind, IStatObject, IBuffContainer
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
            if(_curNode != null)
            {
                foreach (TileNode node in _curNode.neighborNodeDic.Values)
                {
                    if (node.environment != null && node.environment is IManaSupply supply)
                        curMana += supply.manaValue;
                }
            }

            roomManaProperty.Value = curMana;
            return curMana;
        }
    }

    public bool spawnLock = false;

    private int supplyMana = 1;
    public int SupplyMana
    {
        get
        {
            int curMana = supplyMana;
            bool isPath = tileType == TileType.Path;
            if(isPath)
            {
                if (isUpgraded)
                    curMana++;
                foreach (TileNode node in _curNode.neighborNodeDic.Values)
                {
                    if (node.environment != null && node.environment is IManaSupply supply)
                        curMana += supply.manaValue;
                }
            }

            return curMana;
        }
    }

    public TileType _TileType { get => tileType; }

    public List<Direction> PathDirection { get => pathDirection; }
    public List<Direction> RoomDirection { get => roomDirection; }

    private TileNode _curNode;

    public TileNode curNode { get => _curNode; set => _curNode = value; }

    private bool removable = false;
    public bool IsRemovable
    {
        get
        {
            bool isRemovable = removable && !isDormant;
            //foreach(var room in NodeManager.Instance.roomTiles)
            //{
            //    if(room._IncludeRooms.Count > 1 && room._IncludeRooms.Contains(this))
            //    {
            //        isRemovable = false;
            //        break;
            //    }
            //}

            return isRemovable;
        }

        set => removable = value;
    }

    public bool IsCharacterOnIt { get => GameManager.Instance.IsAdventurererOnTile(_curNode) || GameManager.Instance.IsMonsterOnTile(_curNode); }

    private bool movable = false;

    private bool isDormant = false;

    public bool IsDormant { get => isDormant; }
    public bool Movable { get => movable && !GameManager.Instance.moveLock; set { movable = value; } }

    public bool waitToMove = false;

    public Tile twin = null;

    public Trap trap { get => _objectKind is Trap trap ? trap : null; }
    public MonsterSpawner spawner { get => _objectKind is MonsterSpawner spawner ? spawner : null; }

    private IObjectKind _objectKind;
    public IObjectKind objectKind { get => _objectKind; }

    public bool IsBigRoom = false;

    private Renderer tileRenderer;

    protected bool isTwin = false;
    
    private readonly Direction[] RoomRotations = { Direction.None, Direction.Left, Direction.LeftUp, Direction.RightUp, Direction.Right, Direction.RightDown, Direction.LeftDown };

    public ReactiveProperty<bool> _isUpgraded { get; private set; } = new ReactiveProperty<bool>(false);
    public bool isUpgraded { get => _isUpgraded.Value; set => _isUpgraded.Value = value; }


    TileNode prevNode = null;

    private Dictionary<TileNode, int> availableNodeDic = new Dictionary<TileNode, int>();
    public HashSet<TileNode> availableNodes { get => availableNodeDic.Keys.ToHashSet();}

    private CompleteRoom _curRoom;
    public CompleteRoom curRoom { get => _curRoom; set => _curRoom = value; }

    [SerializeField]
    FMODUnity.EventReference rotateSound;
    [SerializeField]
    FMODUnity.EventReference buildSound;
    [SerializeField]
    FMODUnity.EventReference destroySound;

    List<ITileEffect> _curTileEffects = new List<ITileEffect>();
    public List<ITileEffect> curTileEffects => _curTileEffects;

    public List<IBuffEffectInfo> effectInfos
    {
        get
        {
            var list = new List<IBuffEffectInfo>();
            foreach(var effect in curNode.curNodeEffects)
            {
                if(effect is IBuffEffectInfo info)
                    list.Add(info);
            }

            foreach (var effect in curTileEffects)
            {
                if (effect is IBuffEffectInfo info)
                    list.Add(info);
            }

            return list;
        }
    }

    public void ExcuteBattlerEnterEffect(Battler target)
    {
        foreach (var effect in curNode.curNodeEffects)
        {
            if (effect is IBattlerEnterEffect enterEffect)
                enterEffect.Effect(target);
        }

        foreach (var effect in _curTileEffects)
        {
            if (effect is IBattlerEnterEffect enterEffect)
                enterEffect.Effect(target);
        }
    }

    public void ExcuteSetObjectEffect(IObjectKind target)
    {
        foreach (var effect in curNode.curNodeEffects)
        {
            if (effect is IObjectSetEffect objectSetEffect)
                objectSetEffect.Effect(target);
        }

        foreach (var effect in _curTileEffects)
        {
            if (effect is IObjectSetEffect objectSetEffect)
                objectSetEffect.Effect(target);
        }
    }

    public int GetAvailableCount(TileNode node)
    {
        if (!availableNodeDic.ContainsKey(node))
            return 0;

        return availableNodeDic[node];
    }

    private bool HasEnvironmentTileInNeighbors(TileNode node, IEnumerable<Direction> directions)
    {
        foreach (Direction dir in directions)
        {
            if (node.neighborNodeDic[dir] != null && node.neighborNodeDic[dir].environment != null)
                return true;
        }
        return false;
    }

    public void UpdateAvailableNode(HashSet<TileNode> targetNodes, bool includeDormant = true)
    {
        availableNodeDic.Clear();
        for (int i = 0; i < 6; i++)
        {
            foreach(TileNode node in targetNodes)
            {
                if (node == null)
                    continue;

                if (!node.IsConnected(pathDirection, roomDirection, includeDormant))
                    continue;

                if (HasEnvironmentTileInNeighbors(node, pathDirection) || HasEnvironmentTileInNeighbors(node, roomDirection))
                    continue;

                if (!availableNodeDic.ContainsKey(node))
                    availableNodeDic[node] = 0;
                availableNodeDic[node]++;
            }
            RotateTile();
        }
    }

    public int GetUnClosedCount()
    {
        // 불완전 연결인 상태인 타일 개수를 반환하는 함수
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
            _curNode.tileKind = null;
            NodeManager.Instance.SetActiveNode(_curNode, false);
        }

        transform.SetParent(nextNode.transform, false);

        _curNode = nextNode;
        NodeManager.Instance.AddSightNode(_curNode);
        //_curNode?.SetFog(false);
        if (isActive && !NodeManager.Instance._ActiveNodes.Contains(nextNode))
            NodeManager.Instance.SetActiveNode(nextNode, true);

        nextNode.tileKind = this;
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

        NodeManager.Instance.IncreaseNodeVersion();
        NodeManager.Instance.UpdateMinMaxRowCol(_curNode.row, _curNode.col);
        //NodeManager.Instance.UpdateSightNode();
        GameManager.Instance.CheckBattlerCollapsed();
        CheckDevilDisconnection();

        if(GameManager.Instance.IsInit)
        {
            //AudioManager.Instance.Play2DSound("FistHitDoor_ZA01.261", SettingManager.Instance._FxVolume);
            RuntimeManager.PlayOneShot(buildSound);
        }

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
        //현재 노드의 pathDirection과 roomDirection을 60도 회전하는 함수
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
        NodeManager.Instance.SetGuideState(GuideState.Tile, twin);

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
        //마왕 타일과 연결된 상태라면 true
        if (NodeManager.Instance.FindPath(curNode) != null)
            return true;

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
            

            // 보상획득
            NodeManager.Instance.dormantTile.Remove(this);
            //GameManager.Instance.gold += 100;
            //IRewardObject rewardObject = GetComponentInChildren<IRewardObject>();
            //rewardObject?.GetReward();
        }
    }

    public void SetObject(IObjectKind obj)
    {
        _objectKind = obj;

        ExcuteSetObjectEffect(obj);
    }

    public void RemoveObject()
    {
        _objectKind = null;
    }

    private void CheckDevilDisconnection()
    {
        //비활성 타일이거나 게임이 시작되지 않았거나 현재속도가 0이면 체크 패스
        if (isDormant || !GameManager.Instance.IsInit || GameManager.Instance.timeScale == 0)
            return;

        if (NodeManager.Instance.FindPath(NodeManager.Instance.startPoint) == null)
        {
            string desc = DataManager.Instance.GetDescription("announce_ingame_tileconnect");
            GameManager.Instance.popUpMessage?.ToastMsg(desc);
            GameManager.Instance.speedController.SetSpeedZero();
        }
    }

    public void SetRoom(CompleteRoom room)
    {
        _curRoom = room;
        if (_objectKind is MonsterSpawner spawner)
        {
            spawner.isSpawnLocked = false;
        }
    }

    private void DetructRoom()
    {
        if (_curRoom == null)
            return;

        _curRoom = null;
        if(_objectKind is MonsterSpawner spawner)
        {
            spawner.isSpawnLocked = true;
        }
    }

    private void DestructRoomCheck()
    {
        if (_curRoom == null)
            return;

        CompleteRoom room = _curRoom;
        foreach(Tile tile in room._IncludeRooms)
        {
            tile.DetructRoom();
        }

        NodeManager.Instance.roomTiles.Remove(room);
    }

    public void RemoveTile()
    {
        if (isUpgraded && tileType == TileType.Path)
            DownGradeCheck();

        if (tileType is TileType.Room or TileType.Door)
            DestructRoomCheck();

        NodeManager.Instance.SetActiveNode(_curNode, false);
        InputManager.Instance.ResetTileClick();
        _curNode.tileKind = null;
        tileAnimator.SetTrigger("Destroy");

        //AudioManager.Instance.Play2DSound("FistHitDoor_ZA01.262", SettingManager.Instance._FxVolume);
        FMODUnity.RuntimeManager.PlayOneShot(destroySound);

        GameManager.Instance.gold += PassiveManager.Instance._TileDesturctIncome;
        Destroy(this.gameObject, 1.0f);

        NodeManager.Instance.RemoveTile(this);
        GameManager.Instance.CheckBattlerCollapsed();
        GameManager.Instance.UpdateTotalMana();
        CheckDevilDisconnection();
    }

    private void DownGradeCheck()
    {
        HashSet<Tile> checkedTile = new HashSet<Tile>();
        foreach(var dir in pathDirection)
        {
            TileNode node = curNode.neighborNodeDic[dir];
            if (node.curTile == null || node.curTile._TileType != TileType.Path)
                continue;

            if (checkedTile.Contains(node.curTile))
                continue;

            List<Tile> connectedTiles = UtilHelper.GetPathCount(node.curTile);
            foreach(var tile in connectedTiles)
            {
                checkedTile.Add(tile);
                if(connectedTiles.Count < 5)
                {
                    TileUpgrader upgrader = tile.GetComponent<TileUpgrader>();
                    upgrader?.DownGradeTile();
                }
            }
        }
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
        tile.isUpgraded = isUpgraded;

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

        foreach(var tile in targetNode.neighborNodeDic.Values)
        {
            //if(tile.curTile)
        }

        NodeManager.Instance.SetTile(this);
    }

    public void AutoRotate(TileNode curNode, bool isReversed = false)
    {
        int count = 0;
        while (!curNode.IsConnected(pathDirection, roomDirection) || HasEnvironmentTileInNeighbors(curNode, pathDirection) || HasEnvironmentTileInNeighbors(curNode, roomDirection))
        {
            RotateTile(isReversed);
            count++;
            if (count > 6)
                return;
        }

        if (GameManager.Instance.IsInit)
        {
            //AudioManager.Instance.Play2DSound("FistHitDoor_ZA01.261", SettingManager.Instance._FxVolume);
            RuntimeManager.PlayOneShot(rotateSound);
        }
    }

    public void RotateToNext(TileNode curNode, bool isReversed = false)
    {
        if (!availableNodeDic.ContainsKey(curNode) || availableNodeDic[curNode] < 2)
            return;

        RotateTile(isReversed);
        AutoRotate(curNode, isReversed);
    }

    private bool CancelInput()
    {
        return Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(SettingManager.Instance.key_CancelControl._CurKey)
            || Input.GetKeyDown(SettingManager.Instance.key_Deploy._CurKey) || Input.GetKeyDown(SettingManager.Instance.key_Research._CurKey) || Input.GetKeyDown(SettingManager.Instance.key_Shop._CurKey)
            || Input.GetKeyDown(KeyCode.Escape) || GameManager.Instance.isPause;
    }

    protected virtual void Update()
    {
        if (isTwin)
            return;

        //MonsterOutCheck();
        if(waitToMove)
        {
            TileNode curNode = TileMoveCheck();
            if(prevNode != curNode)
            {
                prevNode = curNode;
                if (curNode != null && curNode.setAvail)
                {
                    twin.AutoRotate(curNode);
                    //AudioManager.Instance.Play2DSound("Card_Tile_E", SettingManager.Instance._FxVolume);
                }
            }

            if (Input.GetKeyDown(SettingManager.Instance.key_RotateRight._CurKey))
            {
                RuntimeManager.PlayOneShot(rotateSound);
                //AudioManager.Instance.Play2DSound("Card_Tile_E", SettingManager.Instance._FxVolume);
                twin.RotateToNext(curNode);
            }
            else if (Input.GetKeyDown(SettingManager.Instance.key_RotateLeft._CurKey))
            {
                RuntimeManager.PlayOneShot(rotateSound);
                //AudioManager.Instance.Play2DSound("Card_Tile_Q", SettingManager.Instance._FxVolume);
                twin.RotateToNext(curNode, true);
            }

            if (IsCharacterOnIt || CancelInput())
            {
                EndMoveing();
            }
            else if (Input.GetKeyUp(SettingManager.Instance.key_BasicControl._CurKey) && curNode != null)
            {
                EndMove(curNode);
            }
        }
    }

    public string GetStat(StatType statType)
    {
        switch (statType)
        {
            case StatType.Mana:

                int curMana = SupplyMana;
                if (curMana < supplyMana)
                    return $"<color=red>{curMana}</color>";
                else if(curMana >  supplyMana)
                    return $"<color=green>{curMana}</color>";
                else 
                    return $"{curMana}";
            case StatType.BuffTexts:
                if (effectInfos.Count == 0)
                    return null;

                StringBuilder sb = new StringBuilder();
                foreach(var item in effectInfos)
                {
                    if (string.IsNullOrEmpty(item.descKey))
                        continue;
                    var info = DataManager.Instance.GetDescription(item.descKey).Split('$');
                    sb.Append(info[0]);
                    if (info.Length > 1)
                    {
                        sb.Append(item.effectValue);
                        sb.Append(info[1]);
                    }
                    sb.Append('\n');
                }
                if(sb.Length > 0)
                    sb.Remove(sb.Length - 1, 1);

                return sb.ToString();
            default:
                return null;
        }
    }
}
