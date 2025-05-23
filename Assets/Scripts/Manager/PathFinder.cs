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
            //Ÿ�Ͽ� �ش��ϴ� ��尡 ���³�忡 �̹� �ִ��� Ȯ��
            if (IsInNode(tile, openNode))
            {
                Node node = FindNode(tile, openNode);
                // �̹� ����Ʈ�� �ִٸ�, ���ο����� G�ڽ�Ʈ�� �� �۴ٸ� ��屳ü
                if (FindNode(curTile, openNode).gCost > neighborNode.gCost)
                {
                    openNode.Remove(node);
                    openNode.Add(neighborNode);
                }
            }
            else //���³�忡 ���ٸ� �߰�
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
            //Ÿ�Ͽ� �ش��ϴ� ��尡 ���³�忡 �̹� �ִ��� Ȯ��
            if (IsInNode(tile, openNode))
            {
                Node node = FindNode(tile, openNode);
                // �̹� ����Ʈ�� �ִٸ�, ���ο����� G�ڽ�Ʈ�� �� �۴ٸ� ��屳ü
                if (FindNode(curTile, openNode).gCost > neighborNode.gCost)
                {
                    openNode.Remove(node);
                    openNode.Add(neighborNode);
                }
            }
            else //���³�忡 ���ٸ� �߰�
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
                return null; // �� ����
            }

            // ���³��� �� ���� F�ڽ�Ʈ�� ���� ��带 close��忡 �߰��ϰ�, open��忡�� ����
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
