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
        gCost = PathFinder.GetGCost(prevNode, startTile, closedNode);
        HCost = PathFinder.GetHCost(tileNode.transform.position, endTile.transform.position);
        FCost = gCost + HCost;
        this.prevNode = prevNode;
    }
}

public static class PathFinder
{

    public static float GetHCost(Vector3 curPos, Vector3 endPos)
    {
        // �����Ÿ� = �����Ÿ�
        return (curPos - endPos).magnitude;
    }

    // ��ǥ�Ÿ� -> ������κ��� �����Ÿ��� ����尡 ���۳���϶����� ����
    public static float GetGCost(TileNode prevTile, TileNode startTile, List<Node> closedNode)
    {
        float dist = 1;
        //����� prevTile�� ��� ã��, ����� prevTile�� startTile�̸� ��ȯ
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

    private static void CheckNodeOpened(TileNode curTile, TileNode startTile, TileNode endTile, List<Node> openNode, List<Node> closedNode, out List<Node> resultOpenNode)
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
        resultOpenNode = openNode;
    }

    private static void CheckNode(TileNode curTile, TileNode startTile, TileNode endTile, List<Node> openNode, List<Node> closedNode, out List<Node> resultOpenNode)
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
        resultOpenNode = openNode;
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
            CheckNode(curTile, startTile, endTile, openNode, closedNode, out openNode);
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

    public static float GetBattlerDistance(Battler origin, Battler target)
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
