using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UniRx;

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

public class Tile : MonoBehaviour
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
    public int RoomMana { get => roomMana; }

    public TileType _TileType { get => tileType; }

    public List<Direction> PathDirection { get => pathDirection; }
    public List<Direction> RoomDirection { get => roomDirection; }

    public TileNode curNode;

    private bool removable = false;
    public bool IsRemovable { get { return !removable || isDormant ? false : true; } }

    public bool IsRemovableNow
    {
        get
        {
            bool remove = removable;

            if (remove)
                remove = !GameManager.Instance.IsAdventurererOnTile(curNode);
            if (remove)
                remove = !GameManager.Instance.IsMonsterOnTile(curNode);

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
                canMove = !GameManager.Instance.IsAdventurererOnTile(curNode);
            if (canMove)
                canMove = !GameManager.Instance.IsMonsterOnTile(curNode);
            return canMove;
        }
    }

    public bool waitToMove = false;

    public Tile twin = null;

    public Trap trap = null;

    private MonsterSpawner spawner;
    public MonsterSpawner _Spanwer { get => spawner; }
    public bool HaveSpawner { get { return spawner == null ? false : true; } }

    public bool IsBigRoom = false;

    private Renderer tileRenderer;

    private bool isTwin = false;
    
    private readonly Direction[] RoomRotations = { Direction.None, Direction.Left, Direction.LeftUp, Direction.RightUp, Direction.Right, Direction.RightDown, Direction.LeftDown };

    public int GetUnClosedCount()
    {
        // �ҿ��� ������ ������ Ÿ�� ������ ��ȯ�ϴ� �Լ�
        if (curNode == null)
            return -1;

        int count = 0;
        count += UtilHelper.GetDirectionUnClosed(curNode, PathDirection);
        count += UtilHelper.GetDirectionUnClosed(curNode, RoomDirection, true);

        return count;
    }

    public void MoveTile(TileNode nextNode, bool isActive = true)
    {
        //curNode?.SetFog(true);
        if(curNode != null)
        {
            NodeManager.Instance.RemoveSightNode(curNode);
            curNode.curTile = null;
            NodeManager.Instance.SetActiveNode(curNode, false);
        }

        transform.SetParent(nextNode.transform, false);

        curNode = nextNode;
        NodeManager.Instance.AddSightNode(curNode);
        //curNode?.SetFog(false);
        if (isActive && !NodeManager.Instance._ActiveNodes.Contains(nextNode))
            NodeManager.Instance.SetActiveNode(nextNode, true);

        nextNode.curTile = this;
        transform.position = nextNode.transform.position;
        NodeManager.Instance.ExpandEmptyNode(nextNode, 4);

        if (tileType == TileType.End)
        {
            NodeManager.Instance.endPoint = nextNode;
            if(GameManager.Instance.king != null)
                GameManager.Instance.king.SetTile(nextNode);
        }

        NodeManager.Instance.DormantTileCheck();
        GameManager.Instance.UpdateTotalMana();

        if(GameManager.Instance.IsInit && !GameManager.Instance.speedController.Is_Game_Continuable())
            GameManager.Instance.speedController.SetSpeedZero();

        if (isActive)
        {
            if(tileType == TileType.Room_Single || tileType == TileType.Room || tileType == TileType.Door)
                NodeManager.Instance.RoomCheck(this);
        }

        NodeManager.Instance.UpdateMinMaxRowCol(curNode.row, curNode.col);
        //NodeManager.Instance.UpdateSightNode();
    }

    private void SetTileVisible(bool value)
    {
        if (tileRenderer == null)
            tileRenderer = GetComponentInChildren<Renderer>();
        if (tileRenderer == null) return;

        tileRenderer.enabled = value;
    }

    private void UpdateMoveTilePos(TileNode curNode)
    {
        if (curNode != null && NodeManager.Instance.emptyNodes.Contains(curNode) || curNode == this.curNode)
            twin.transform.position = curNode.transform.position;
        else
            twin.transform.position = new Vector3(0, 10000, 0);

        SetTileVisible(curNode != this.curNode);
    }

    public void EndMoveing(bool resetNode = true)
    {
        twin.gameObject.SetActive(false);
        waitToMove = false;
        InputManager.Instance.settingCard = false;
        InputManager.Instance.movingTile = false;
        InputManager.Instance.ResetTileClick();
        if (resetNode)
            NodeManager.Instance.SetActiveNode(this.curNode, true);
        NodeManager.Instance.SetGuideState(GuideState.None);
        SetTileVisible(true);
    }

    public void EndMove(TileNode curNode)
    {
        bool resetNode = true;
        if (curNode != null && curNode.setAvail)
        {
            transform.rotation = twin.transform.rotation;
            rotationCount.Value = twin.rotationCount.Value;
            pathDirection = new List<Direction>(twin.pathDirection);
            roomDirection = new List<Direction>(twin.roomDirection);

            NodeManager.Instance.SetActiveNode(this.curNode, false);
            NodeManager.Instance.SetActiveNode(curNode, true);
            MoveTile(curNode);

            AudioManager.Instance.Play2DSound("Click_tile", SettingManager.Instance._FxVolume);
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

    private int rotation = 0;
    public int _Rotation { get => rotation; }

    public void RotateTile(bool reverse = false)
    {
        pathDirection = RotateDirection(pathDirection, reverse);
        roomDirection = RotateDirection(roomDirection, reverse);
        RotateDirection(reverse);
        NodeManager.Instance.SetGuideState(GuideState.Tile, this);

        if (reverse)
            rotation--;
        else
            rotation++;
        if (rotation < 0)
            rotation = 5;
        rotation %= 6;
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
            twin.isTwin = true;
            twin.waitToMove = false;
        }
        twin.gameObject.SetActive(true);
        twin.pathDirection = new List<Direction>(pathDirection);
        twin.roomDirection = new List<Direction>(roomDirection);
        twin.transform.rotation = transform.rotation;
        twin.rotationCount.Value = rotationCount.Value;
        NodeManager.Instance.SetActiveNode(this.curNode, false);
    }

    public void ReadyForMove()
    {
        SetTwin();
        InputManager.Instance.movingTile = true;
        switch (tileType)
        {
            case TileType.End:
                NodeManager.Instance.SetGuideState(GuideState.Tile, this);
                break;
            default:
                NodeManager.Instance.SetGuideState(GuideState.Tile, this);
                break;
        }

        curNode.SetAvail(true);
        waitToMove = true;

        AudioManager.Instance.Play2DSound("Click_tile", SettingManager.Instance._FxVolume);
    }

    public TileNode TileMoveCheck()
    {
        TileNode curNode = UtilHelper.RayCastTile();
        UpdateMoveTilePos(curNode);
        return curNode;
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
            TileNode target = curNode.neighborNodeDic[direction];
            if (target == null || target.curTile == null)
                continue;
            if (UtilHelper.IsTileConnected(curNode, direction, target.curTile.PathDirection))
                return true;
            if (UtilHelper.IsTileConnected(curNode, direction, target.curTile.PathDirection))
                return true;
        }

        return false;
    }

    public void DormantAwakeCheck()
    {
        if(DormantAwake())
        {
            isDormant = false;
            if(!GameManager.Instance.speedController.Is_Tile_Connected(this.curNode))
            {
                isDormant = true;
                return;
            }

            movable = false;
            

            // ����ȹ��
            NodeManager.Instance.dormantTile.Remove(this);
            GameManager.Instance.gold += 100;
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
        NodeManager.Instance.SetActiveNode(curNode, false);
        InputManager.Instance.ResetTileClick();
        tileAnimator.SetTrigger("Destroy");

        GameManager.Instance.gold += PassiveManager.Instance._TileDesturctIncome;
        trap?.DestroyTrap();
        Destroy(this.gameObject, 1.0f);

        NodeManager.Instance.RemoveTile(this);
    }

    public TileData GetTileData()
    {
        TileData tile = new TileData();
        tile.id = name.ToString().Replace("(Clone)", "");
        tile.row = curNode.row;
        tile.col = curNode.col;
        tile.rotation = rotation;
        tile.isDormant = isDormant;
        tile.isRemovable = removable;
        tile.trapId = trap != null ? trap.BattlerID : "";
        
        if(spawner != null)
        {
            tile.spawnerId = spawner._TargetName;
            tile.spawnerCool = spawner._CurCoolTime;
        }

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
            NodeManager.Instance.dormantTile.Add(this);
        if(playAnim)
            tileAnimator?.SetTrigger("Set");

        NodeManager.Instance.SetTile(this);
    }

    private void Update()
    {
        if (isTwin)
            return;

        //MonsterOutCheck();

        if(waitToMove)
        {
            TileNode curTile = TileMoveCheck();
            if (Input.GetKeyDown(SettingManager.Instance.key_RotateRight._CurKey))
                twin.RotateTile();
            else if(Input.GetKeyDown(SettingManager.Instance.key_RotateLeft._CurKey))
                twin.RotateTile(true);
            if (!MovableNow || Input.GetKeyUp(SettingManager.Instance.key_CancelControl._CurKey))
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
