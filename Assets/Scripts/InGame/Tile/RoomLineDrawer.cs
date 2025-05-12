using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class RoomLineDrawer : MonoBehaviour
{
    private LineRenderer _lineRenderer;
    readonly List<TileEdgeDirection> directions = new List<TileEdgeDirection>() { TileEdgeDirection.LeftUp, TileEdgeDirection.LeftDown, TileEdgeDirection.Down, TileEdgeDirection.RightDown, TileEdgeDirection.RightUp, TileEdgeDirection.Up };
    [SerializeField]
    private float blinkSpeed = 1f;

    private async UniTaskVoid FadeEffect()
    {
        if (_lineRenderer == null)
            return;
        Material target = _lineRenderer.material;
        target.SetColor("_Color", Color.white);
        float curAlpha = 1f;
        while(gameObject.activeSelf)
        {
            while(curAlpha > 0.3f)
            {
                curAlpha -= Time.deltaTime * blinkSpeed;
                target.SetColor("_Color", new Color(1, 1, 1, curAlpha));
                await UniTask.Yield(cancellationToken: gameObject.GetCancellationTokenOnDestroy());
            }

            while (curAlpha < 1f)
            {
                curAlpha += Time.deltaTime * blinkSpeed;
                target.SetColor("_Color", new Color(1, 1, 1, curAlpha));
                await UniTask.Yield(cancellationToken: gameObject.GetCancellationTokenOnDestroy());
            }
        }
    }

    private void OnEnable()
    {
        FadeEffect().Forget();
    }

    public void Init(List<Tile> targetRoom)
    {
        _lineRenderer = GetComponent<LineRenderer>();
        if (targetRoom.Count == 1)
        {
            TileEdgeGuide tileEdge = targetRoom[0].GetComponentInChildren<TileEdgeGuide>();
            _lineRenderer.positionCount = 7;
            int point = 0;
            foreach(var kvp in tileEdge.tileDirectionPos)
            {
                _lineRenderer.SetPosition(point, kvp.Value.position);
                point++;
            }
            _lineRenderer.SetPosition(point, tileEdge.tileDirectionPos[TileEdgeDirection.LeftUp].position);

            gameObject.SetActive(false);
            return;
        }

        Dictionary<Tile, Dictionary<TileEdgeDirection, Tile>> tileEdgeDic = CalculateEdgeStatus(targetRoom);
        DrawRoomOutline(tileEdgeDic);

        gameObject.SetActive(false);
    }

    private TileEdgeDirection GetNextDireciton(TileEdgeDirection curDirection)
    {
        int index = (int)curDirection + 1;
        if (index == 6)
            index = 0;
        return directions[index];
    }

    private void DrawRoomOutline(Dictionary<Tile, Dictionary<TileEdgeDirection, Tile>> tileEdgeDic)
    {
        List<Transform> edgePositions = new List<Transform>();
        TileEdgeDirection startDirection = TileEdgeDirection.None;
        Tile startTile = null;
        //시작점 선정
        foreach(var tileEdgeKvp in tileEdgeDic)
        {
            foreach(var kvp in tileEdgeKvp.Value)
            {
                if(kvp.Value == tileEdgeKvp.Key) // 접점 0개
                {
                    startDirection = kvp.Key;
                    startTile = tileEdgeKvp.Key;
                    break;
                }
            }
            if (startTile != null)
                break;
        }

        if(startTile == null)
        {
            Debug.Log("room outliner failed to find startPoint");
            return;
        }

        
        TileEdgeDirection curDirection = startDirection;
        Tile curTile = startTile;
        TileEdgeGuide tileEdgeGuide = curTile.GetComponentInChildren<TileEdgeGuide>();
        edgePositions.Add(tileEdgeGuide.tileDirectionPos[curDirection]);
        curDirection = GetNextDireciton(curDirection);
        if (tileEdgeDic[curTile][curDirection] != curTile)
        {
            Tile prevTile = curTile;
            curTile = tileEdgeDic[curTile][curDirection]; //접점이 다음타일일경우 이동
            Direction tileDirection = Direction.None;
            foreach(var kvp in prevTile.curNode.neighborNodeDic)
            {
                if(kvp.Value.curTile != null && kvp.Value.curTile == curTile)
                {
                    tileDirection = kvp.Key;
                    break;
                }
            }
            curDirection = GetReversedDirection(curDirection, tileDirection);
        }

        int loopCount = 0;
        while(curTile != startTile || curDirection != startDirection)
        {
            loopCount++;
            
            tileEdgeGuide = curTile.GetComponentInChildren<TileEdgeGuide>();
            edgePositions.Add(tileEdgeGuide.tileDirectionPos[curDirection]);
            curDirection = GetNextDireciton(curDirection);
            if (tileEdgeDic[curTile][curDirection] != curTile)
            {
                Tile prevTile = curTile;
                curTile = tileEdgeDic[curTile][curDirection]; //접점이 다음타일일경우 이동
                Direction tileDirection = Direction.None;
                foreach (var kvp in prevTile.curNode.neighborNodeDic)
                {
                    if (kvp.Value.curTile != null && kvp.Value.curTile == curTile)
                    {
                        tileDirection = kvp.Key;
                        break;
                    }
                }
                curDirection = GetReversedDirection(curDirection, tileDirection);
            }

            if (loopCount >= 100000000)
            {
                Debug.Log("room outliner failed to compelete line");
                return;
            }
        }

        //시작 포인트로 마무리
        tileEdgeGuide = curTile.GetComponentInChildren<TileEdgeGuide>();
        edgePositions.Add(tileEdgeGuide.tileDirectionPos[curDirection]);

        _lineRenderer.positionCount = edgePositions.Count;
        for(int i = 0; i < edgePositions.Count; i++)
        {
            edgePositions[i].parent.rotation = Quaternion.identity;
            _lineRenderer.SetPosition(i, edgePositions[i].position);
        }
    }

    private Dictionary<Tile, Dictionary<TileEdgeDirection, Tile>> CalculateEdgeStatus(List<Tile> targetRoom)
    {
        Dictionary<Tile, Dictionary<TileEdgeDirection, Tile>> tileEdgeDic = new Dictionary<Tile, Dictionary<TileEdgeDirection, Tile>>();
        HashSet<Tile> roomTiles = targetRoom.ToHashSet();
        foreach (var room in targetRoom)
        {
            tileEdgeDic[room] = new Dictionary<TileEdgeDirection, Tile>();
            foreach (var dir in directions)
                tileEdgeDic[room][dir] = room;
        }

        foreach (var room in targetRoom)
        {
            foreach (var neighborKvp in room.curNode.neighborNodeDic)
            {
                Tile neighborTile = neighborKvp.Value.curTile;
                if (neighborTile == null || !roomTiles.Contains(neighborTile))
                    continue;

                Direction tileDir = neighborKvp.Key;
                foreach (var dir in directions)
                {
                    if (IsValidDirection(dir, tileDir)) // 접점에있는 타일일경우 접점이 0(본인)일경우 접점을 1로(이웃타일) 갱신, 이미 접점이1(이웃타일)일경우 접점을 2로 갱신(null)
                        tileEdgeDic[room][dir] = tileEdgeDic[room][dir] == room ? neighborTile : null;
                }
            }
        }

        return tileEdgeDic;
    }

    private bool IsValidDirection(TileEdgeDirection targetDir, Direction tileDir)
    {
        switch(tileDir)
        {
            case Direction.LeftUp:
                return targetDir is TileEdgeDirection.Up or TileEdgeDirection.LeftUp;
            case Direction.RightUp:
                return targetDir is TileEdgeDirection.Up or TileEdgeDirection.RightUp;
            case Direction.Left:
                return targetDir is TileEdgeDirection.LeftUp or TileEdgeDirection.LeftDown;
            case Direction.Right:
                return targetDir is TileEdgeDirection.RightUp or TileEdgeDirection.RightDown;
            case Direction.LeftDown:
                return targetDir is TileEdgeDirection.LeftDown or TileEdgeDirection.Down;
            case Direction.RightDown:
                return targetDir is TileEdgeDirection.RightDown or TileEdgeDirection.Down;
        }
        return false;
    }

    private TileEdgeDirection GetReversedDirection(TileEdgeDirection target, Direction direction)
    {
        switch(direction)
        {
            case Direction.LeftUp:
                if (target == TileEdgeDirection.Up)
                    return TileEdgeDirection.RightDown;
                else if (target == TileEdgeDirection.LeftUp)
                    return TileEdgeDirection.Down;
                break;
            case Direction.RightUp:
                if (target == TileEdgeDirection.Up)
                    return TileEdgeDirection.LeftDown;
                else if (target == TileEdgeDirection.RightUp)
                    return TileEdgeDirection.Down;
                break;
            case Direction.Left:
                if(target == TileEdgeDirection.LeftUp)
                    return TileEdgeDirection.RightUp;
                else if(target == TileEdgeDirection.LeftDown)
                    return TileEdgeDirection.RightDown;
                break;
            case Direction.Right:
                if (target == TileEdgeDirection.RightUp)
                    return TileEdgeDirection.LeftUp;
                else if (target == TileEdgeDirection.RightDown)
                    return TileEdgeDirection.LeftDown;
                break;
            case Direction.LeftDown:
                if (target == TileEdgeDirection.LeftDown)
                    return TileEdgeDirection.Up;
                else if (target == TileEdgeDirection.Down)
                    return TileEdgeDirection.RightUp;
                break;
            case Direction.RightDown:
                if (target == TileEdgeDirection.RightDown)
                    return TileEdgeDirection.Up;
                else if (target == TileEdgeDirection.Down)
                    return TileEdgeDirection.LeftUp;
                break;
        }
        return TileEdgeDirection.None;
    }
}
