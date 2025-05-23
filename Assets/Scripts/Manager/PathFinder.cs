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
        if (prevNode == startTile)
            gCost = 1;
        else
        {
            gCost = int.MaxValue;
            for (int i = closedNode.Count - 1; i >= 0; i--)
            {
                if (closedNode[i].tileNode == prevNode)
                {
                    gCost = closedNode[i].gCost + 1;
                    break;
                }
            }
        }
        HCost = (tileNode.transform.position - endTile.transform.position).magnitude;
        FCost = gCost + HCost;
        this.prevNode = prevNode;
    }
}

public static class PathFinder
{
    private static bool IsInNode(TileNode targetTile, List<Node> targetNode)
    {
        foreach (Node node in targetNode)
        {
            if (node.tileNode == targetTile)
                return true;
        }

        return false;
    }

    private static Node FindNode(TileNode targetTile, List<Node> targetNode)
    {
        foreach (Node node in targetNode)
        {
            if (node.tileNode == targetTile)
                return node;
        }

        return new Node();
    }

    private static void CheckNodeOpened(TileNode curTile, TileNode startTile, TileNode endTile, List<Node> openNode, List<Node> closedNode)
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
    }

    private static void CheckNode(TileNode curTile, TileNode startTile, TileNode endTile, List<Node> openNode, List<Node> closedNode)
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
    }

    public static List<TileNode> CalculateFinalPath(TileNode startTile, TileNode endTile, List<Node> closedNode)
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

    public static List<TileNode> FindPath(TileNode startTile, TileNode endTile = null)
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
            CheckNode(curTile, startTile, endTile, openNode, closedNode);
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

    public static int GetNodeDistance(TileNode startTile, TileNode endTile)
    {
        if (startTile == endTile)
            return -1;
        List<Node> openNode = new List<Node>();
        List<Node> closedNode = new List<Node>();
        TileNode curTile = startTile;
        while (curTile != endTile)
        {
            CheckNodeOpened(curTile, startTile, endTile, openNode, closedNode);
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
}
