using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Node
{
    public TileNode tileNode;
    public float gCost;
    public float HCost;
    public float FCost;
    public TileNode prevNode;

    public Node(TileNode tileNode, TileNode prevNode, TileNode startTile, TileNode endTile, List<Node> closedNode)
    {
        this.tileNode = tileNode;
        gCost = PathFinder.Instance.GetGCost(prevNode, startTile, closedNode);
        HCost = PathFinder.Instance.GetHCost(tileNode.transform.position, endTile.transform.position);
        FCost = gCost + HCost;
        this.prevNode = prevNode;
    }
}

public class PathFinder : Singleton<PathFinder>
{
    
    public float GetHCost(Vector3 curPos, Vector3 endPos)
    {
        // 추정거리 = 직선거리
        return (curPos - endPos).magnitude;
    }

    // 목표거리 -> 현재노드로부터 전노드거리를 전노드가 시작노드일때까지 더함
    public float GetGCost(TileNode prevTile, TileNode startTile, List<Node> closedNode)
    {
        float dist = 1;
        //노드중 prevTile을 계속 찾고, 노드의 prevTile이 startTile이면 반환
        //dist = curPaths.Count - 1;
        if(prevTile == startTile)
            return dist;

        int errorStack = 0;
        while(prevTile != startTile)
        {
            foreach (Node node in closedNode)
            {
                if (node.tileNode == prevTile)
                {
                    dist++;
                    prevTile = node.prevNode;
                    break;
                }
            }

            errorStack++;
            if (errorStack > 10000)
                break;
        }

        return dist;
    }

    

    private bool IsInNode(TileNode targetTile, List<Node> targetNode)
    {
        foreach (Node node in targetNode)
        {
            if (node.tileNode == targetTile)
                return true;
        }

        return false;
    }

    private Node FindNode(TileNode targetTile, List<Node> targetNode)
    {
        foreach (Node node in targetNode)
        {
            if (node.tileNode == targetTile)
                return node;
        }

        return new Node();
    }

    private void CheckNodeOpened(TileNode curTile, TileNode startTile, TileNode endTile, List<Node> openNode, List<Node> closedNode, out List<Node> resultOpenNode)
    {
        List<TileNode> neighborTiles = new List<TileNode>(curTile.neighborNodeDic.Values);
        foreach (TileNode tile in neighborTiles)
        {
            if (tile == startTile || IsInNode(tile, closedNode))
                continue;

            //TileNode tempPrevTile = curPaths.;
            Node neighborNode = new Node(tile, curTile, startTile, endTile, closedNode);
            //타일에 해당하는 노드가 오픈노드에 이미 있는지 확인
            if (IsInNode(tile, openNode))
            {
                Node node = FindNode(tile, openNode);
                // 이미 리스트에 있다면, 새로운노드의 G코스트가 더 작다면 노드교체
                if (FindNode(curTile, openNode).gCost > neighborNode.gCost)
                {
                    openNode.Remove(node);
                    openNode.Add(neighborNode);
                }
            }
            else //오픈노드에 없다면 추가
                openNode.Add(neighborNode);
        }
        resultOpenNode = openNode;
    }

    private void CheckNode(TileNode curTile, TileNode startTile, TileNode endTile, List<Node> openNode, List<Node> closedNode, out List<Node> resultOpenNode)
    {
        List<TileNode> neighborTiles = UtilHelper.GetConnectedNodes(curTile);
        foreach(TileNode tile in neighborTiles)
        {
            if (tile == startTile || IsInNode(tile, closedNode))
                continue;

            //TileNode tempPrevTile = curPaths.;
            Node neighborNode = new Node(tile, curTile, startTile, endTile, closedNode);
            //타일에 해당하는 노드가 오픈노드에 이미 있는지 확인
            if (IsInNode(tile, openNode))
            {
                Node node = FindNode(tile, openNode);
                // 이미 리스트에 있다면, 새로운노드의 G코스트가 더 작다면 노드교체
                if (FindNode(curTile, openNode).gCost > neighborNode.gCost)
                {
                    openNode.Remove(node);
                    openNode.Add(neighborNode);
                }
            }
            else //오픈노드에 없다면 추가
                openNode.Add(neighborNode);
        }
        resultOpenNode = openNode;
    }

    public List<TileNode> CalculateFinalPath(TileNode startTile, TileNode endTile, List<Node> closedNode)
    {
        List<TileNode> finalPath = new List<TileNode>();
        finalPath.Add(endTile);

        TileNode prevNode = FindNode(endTile, closedNode).prevNode;
        while(prevNode != startTile)
        {
            finalPath.Add(prevNode);
            prevNode = FindNode(prevNode, closedNode).prevNode;
        }

        return finalPath;
    }

    public List<TileNode> FindPath(TileNode startTile, TileNode endTile = null)
    {
        if(endTile == null)
            endTile = NodeManager.Instance.endPoint;

        if (startTile == endTile)
            return null;

        List<Node> openNode = new List<Node>();
        List<Node> closedNode = new List<Node>();
        TileNode curTile = startTile;
        while (curTile != endTile)
        {
            CheckNode(curTile, startTile, endTile, openNode, closedNode, out openNode);
            if (openNode.Count == 0)
            {
                return null; // 길 없음
            }

            // 오픈노드들 중 가장 F코스트가 낮은 노드를 close노드에 추가하고, open노드에서 제거
            float minFCost = openNode[0].FCost;
            Node minFCostNode = openNode[0];
            foreach (Node node in openNode)
            {
                if (node.FCost < minFCost)
                    minFCostNode = node;
            }
            closedNode.Add(minFCostNode);
            openNode.Remove(minFCostNode);
            curTile = minFCostNode.tileNode;
        }

        List<TileNode> finalNode = CalculateFinalPath(startTile, endTile, closedNode);
        finalNode.Reverse();
        return finalNode;
    }

    public int GetNodeDistance(TileNode startTile, TileNode endTile)
    {
        if (startTile == endTile)
            return -1;
        List<Node> openNode = new List<Node>();
        List<Node> closedNode = new List<Node>();
        TileNode curTile = startTile;
        while (curTile != endTile)
        {
            CheckNodeOpened(curTile, startTile, endTile, openNode, closedNode, out openNode);
            if (openNode.Count == 0)
                return -1;

            float minFCost = openNode[0].FCost;
            Node minFCostNode = openNode[0];
            foreach (Node node in openNode)
            {
                if (node.FCost < minFCost)
                    minFCostNode = node;
            }
            closedNode.Add(minFCostNode);
            openNode.Remove(minFCostNode);
            curTile = minFCostNode.tileNode;
        }

        List<TileNode> finalNode = CalculateFinalPath(startTile, endTile, closedNode);
        return finalNode.Count;
    }

    public float GetBattlerDistance(Battler origin, Battler target)
    {
        if (origin.CurTile == target.CurTile)
        {
            float modifyOrigin = Vector3.Distance(origin.transform.position, origin.CurTile.transform.position);
            float modifyTarget = Vector3.Distance(target.transform.position, target.CurTile.transform.position);
            Direction originBattlerDirection = UtilHelper.CheckClosestDirection(origin.transform.position - origin.CurTile.transform.position);
            Direction targetBattlerDirection = UtilHelper.CheckClosestDirection(target.transform.position - target.CurTile.transform.position);
            if (originBattlerDirection == targetBattlerDirection)
                modifyTarget *= -1f;

            return modifyOrigin + modifyTarget;
        }
        else
        {
            List<TileNode> path = FindPath(origin.CurTile, target.CurTile);
            if (path == null || path.Count <= 0)
                return Mathf.Infinity;
            float distance = path.Count * 1f;

            float modifyOrigin = Vector3.Distance(origin.transform.position, origin.CurTile.transform.position);
            Direction originBattlerDirection = UtilHelper.CheckClosestDirection(origin.transform.position - origin.CurTile.transform.position);
            Direction originPathDirection = origin.CurTile.GetNodeDirection(path[0]);
            if (originBattlerDirection == originPathDirection)
                modifyOrigin *= -1f;

            float modifyTarget = Vector3.Distance(target.transform.position, target.CurTile.transform.position);
            Direction targetBattlerDirection = UtilHelper.CheckClosestDirection(target.CurTile.transform.position - target.transform.position);
            TileNode destPoint = origin.CurTile;
            if (path.Count > 1)
                destPoint = path[path.Count - 2];
            Direction targetPathDirection = destPoint.GetNodeDirection(target.CurTile);
            if (targetBattlerDirection == targetPathDirection)
                modifyTarget *= -1f;

            return distance + modifyOrigin + modifyTarget;
        }
    }
}
