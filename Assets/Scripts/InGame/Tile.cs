using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum TileType
{
    Start,
    Path,
    Room,
    End,
}

public class Tile : MonoBehaviour
{
    [SerializeField]
    private List<Direction> pathDirection = new List<Direction>();
    [SerializeField]
    private List<Direction> roomDirection = new List<Direction>();
    [SerializeField]
    private TileType tileType;

    public TileType _TileType { get => tileType; }

    public List<Direction> PathDirection { get => pathDirection; }
    public List<Direction> RoomDirection { get => roomDirection; }

    public TileNode curNode;

    public bool isActive = false;

    public bool movable = false;

    private bool isDormant = false;

    public bool IsDormant { get => isDormant; }

    public bool Movable
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
    public Monster monster = null;

    public bool IsBigRoom = false;

    private Renderer tileRenderer;

    private bool isTwin = false;

    public int GetUnClosedCount()
    {
        // 불완전 연결인 상태인 타일 개수를 반환하는 함수
        if (curNode == null)
            return -1;

        int count = 0;
        count += UtilHelper.GetDirectionUnClosed(curNode, PathDirection);
        count += UtilHelper.GetDirectionUnClosed(curNode, RoomDirection, true);

        return count;
    }

    public void MoveTile(TileNode nextNode, bool isActive = true)
    {
        if(curNode != null)
        {
            curNode.curTile = null;
            NodeManager.Instance.activeNodes.Remove(curNode);
        }

        transform.SetParent(nextNode.transform, false);

        curNode = nextNode;
        if(isActive && !NodeManager.Instance.activeNodes.Contains(nextNode))
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

        if(GameManager.Instance.IsInit && !GameManager.Instance.speedController.Is_All_Tile_Connected())
            GameManager.Instance.speedController.SetSpeedZero();
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
        if(resetNode)
            NodeManager.Instance.SetActiveNode(this.curNode, true);
        NodeManager.Instance.SetGuideState(GuideState.None);
        SetTileVisible(true);
    }

    private void InstanceTwin()
    {
        twin = Instantiate(this, transform);
        twin.isTwin = true;
        twin.waitToMove = false;
        twin.transform.rotation = transform.rotation;
    }

    public void EndMove(TileNode curNode)
    {
        bool resetNode = true;
        if (curNode != null && curNode.setAvail)
        {
            transform.rotation = twin.transform.rotation;
            pathDirection = new List<Direction>(twin.pathDirection);
            roomDirection = new List<Direction>(twin.roomDirection);

            NodeManager.Instance.SetActiveNode(this.curNode, false);
            NodeManager.Instance.SetActiveNode(curNode, true);
            MoveTile(curNode);

            AudioManager.Instance.Play2DSound("Click_tile", SettingManager.Instance._FxVolume);
            resetNode = false;

            if (SettingManager.Instance.autoPlay == AutoPlaySetting.setTile || SettingManager.Instance.autoPlay == AutoPlaySetting.always)
                GameManager.Instance.speedController.SetSpeedPrev(false);
        }

        EndMoveing(resetNode);
    }

    private void RotateDirection()
    {
        transform.rotation *= Quaternion.Euler(0f, 60f, 0f);
    }

    public List<Direction> RotateDirection(List<Direction> pathDirection)
    {
        //현재 노드의 pathDirection과 roomDirection을 60도 회전하는 함수
        List<Direction> newDirection = new List<Direction>();
        foreach (Direction dir in pathDirection)
        {
            switch (dir)
            {
                case Direction.Left:
                    newDirection.Add(Direction.LeftUp);
                    break;
                case Direction.LeftUp:
                    newDirection.Add(Direction.RightUp);
                    break;
                case Direction.LeftDown:
                    newDirection.Add(Direction.Left);
                    break;
                case Direction.Right:
                    newDirection.Add(Direction.RightDown);
                    break;
                case Direction.RightUp:
                    newDirection.Add(Direction.Right);
                    break;
                case Direction.RightDown:
                    newDirection.Add(Direction.LeftDown);
                    break;
            }
        }

        return newDirection;
    }

    public void RotateTile()
    {
        pathDirection = RotateDirection(pathDirection);
        roomDirection = RotateDirection(roomDirection);
        RotateDirection();
        if(tileType != TileType.End)
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
            InstanceTwin();
            NodeManager.Instance.SetActiveNode(this.curNode, false);
        }
        else if (!twin.gameObject.activeSelf)
        {
            twin.gameObject.SetActive(true);
            twin.transform.rotation = transform.rotation;
            twin.pathDirection = new List<Direction>(pathDirection);
            twin.roomDirection = new List<Direction>(roomDirection);
            NodeManager.Instance.SetActiveNode(this.curNode, false);
        }
    }

    public void ReadyForMove()
    {
        SetTwin();

        switch (tileType)
        {
            case TileType.End:
                UtilHelper.SetAvail(true, NodeManager.Instance.allNodes);
                UtilHelper.SetAvail(false, NodeManager.Instance.activeNodes);
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
        if (monster == null)
            return;

        if ((monster.transform.position - transform.position).magnitude > 0.5f)
            monster = null;
    }

    private bool DormantAwake()
    {
        //연결된 타일 중 active상태인 노드가 있으면 return true
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
            if(!GameManager.Instance.speedController.Is_All_Tile_Connected())
            {
                isDormant = true;
                return;
            }

            movable = false;
            

            // 보상획득
            NodeManager.Instance.dormantTile.Remove(this);
            GameManager.Instance.gold += 100;
        }
    }

    public void Init(TileNode targetNode, bool dormant = false)
    {
        NodeManager.Instance.SetActiveNode(targetNode, true);
        MoveTile(targetNode);
        isDormant = dormant;
        movable = !dormant;
        if(isDormant)
            NodeManager.Instance.dormantTile.Add(this);
    }

    private void Update()
    {
        if (isTwin)
            return;

        MonsterOutCheck();
    }
}
