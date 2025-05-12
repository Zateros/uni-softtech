using System.Collections.Generic;
using System;
using UnityEngine;

public class AStar : MonoBehaviour
{
    private bool _road;
    public void FindPath(PathRequest request, Action<PathResult> callback, bool road)
    {
        _road = road;
        Vector2[] waypoints = new Vector2[0];
        bool pathSuccess = false;
        Vector3Int start = GameManager.Instance.GameTable.WorldToCell(request.pathStart);
        Vector3Int end = GameManager.Instance.GameTable.WorldToCell(request.pathEnd);
        if(!GameManager.Instance.GameTable.IsInBounds(end.x, end.y))
        {
            callback(new PathResult(new Vector2[0], false, request.callback));
            return;
        }
        Node startNode;
        Node targetNode;
        if (road)
        {
            startNode = GameManager.Instance.Roadmap[start.x, start.y];
            targetNode = GameManager.Instance.Roadmap[end.x, end.y];
        }
        else
        {
            startNode = GameManager.Instance.WMap[start.x, start.y];
            targetNode = GameManager.Instance.WMap[end.x, end.y];
        }
        
        startNode.parent = startNode;

        if(!targetNode.passible && !road)
        {
            var WMap = GameManager.Instance.WMap;
            if (GameManager.Instance.GameTable.IsInBounds(end.x - 1, end.y - 1))
            {
                if (WMap[end.x - 1, end.y - 1].passible)
                {
                    targetNode = WMap[end.x - 1, end.y - 1];
                }
            }
            if (GameManager.Instance.GameTable.IsInBounds(end.x - 1, end.y))
            {
                if (WMap[end.x - 1, end.y].passible)
                {
                    targetNode = WMap[end.x - 1, end.y];
                }
            }
            if (GameManager.Instance.GameTable.IsInBounds(end.x - 1, end.y + 1))
            {
                if (WMap[end.x - 1, end.y + 1].passible)
                {
                    targetNode = WMap[end.x - 1, end.y + 1];
                }
            }
            if (GameManager.Instance.GameTable.IsInBounds(end.x, end.y - 1))
            {
                if (WMap[end.x, end.y - 1].passible)
                {
                    targetNode = WMap[end.x, end.y - 1];
                }
            }
            if (GameManager.Instance.GameTable.IsInBounds(end.x, end.y + 1))
            {
                if (WMap[end.x, end.y + 1].passible)
                {
                    targetNode = WMap[end.x, end.y + 1];
                }
            }
            if (GameManager.Instance.GameTable.IsInBounds(end.x + 1, end.y - 1))
            {
                if (WMap[end.x + 1, end.y - 1].passible)
                {
                    targetNode = WMap[end.x + 1, end.y - 1];
                }
            }
            if (GameManager.Instance.GameTable.IsInBounds(end.x + 1, end.y))
            {
                if (WMap[end.x - 1, end.y].passible)
                {
                    targetNode = WMap[end.x - 1, end.y];
                }
            }
            if (GameManager.Instance.GameTable.IsInBounds(end.x + 1, end.y + 1))
            {
                if (WMap[end.x + 1, end.y + 1].passible)
                {
                    targetNode = WMap[end.x + 1, end.y + 1];
                }
            }
        }
        if (startNode.passible && targetNode.passible)
        {
            Heap<Node> openSet = new Heap<Node>(GameManager.Instance.GameTable.Size.x * GameManager.Instance.GameTable.Size.y);
            HashSet<Node> closedSet = new HashSet<Node>();
            openSet.Add(startNode);
            while (openSet.Count > 0)
            {
                Node currentNode = openSet.RemoveFirst();
                closedSet.Add(currentNode);
                if (currentNode == targetNode)
                {
                    pathSuccess = true;
                    break;
                }

                foreach (Node neighbour in GetNeighbours(currentNode))
                {
                    if (!neighbour.passible || closedSet.Contains(neighbour))
                    {
                        continue;
                    }

                    int newMovementCostToNeighbour = (currentNode.gCost + GetDistance(currentNode, neighbour)) * neighbour.weigth;
                    if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                    {
                        neighbour.gCost = newMovementCostToNeighbour;
                        neighbour.hCost = GetDistance(neighbour, targetNode);
                        neighbour.parent = currentNode;

                        if (!openSet.Contains(neighbour))
                            openSet.Add(neighbour);
                        else
                            openSet.UpdateItem(neighbour);
                    }
                }
            }
        }
        if (pathSuccess)
        {
            waypoints = RetracePath(startNode, targetNode);
            pathSuccess = waypoints.Length > 0;
        }
        callback(new PathResult(waypoints, pathSuccess, request.callback));

    }

    int GetDistance(Node nodeA, Node nodeB)
    {
        int dstX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int dstY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

        if (dstX > dstY)
            return 14 * dstY + 10 * (dstX - dstY);
        return 14 * dstX + 10 * (dstY - dstX);
    }

    public List<Node> GetNeighbours(Node node)
    {
        List<Node> neighbours = new List<Node>();

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0)
                    continue;

                int checkX = node.gridX + x;
                int checkY = node.gridY + y;

                if (GameManager.Instance.GameTable.IsInBounds(checkX,checkY))
                {
                    if (_road)
                    {
                        neighbours.Add(GameManager.Instance.Roadmap[checkX, checkY]);
                    }
                    else
                    {
                        neighbours.Add(GameManager.Instance.WMap[checkX, checkY]);
                    }
                }
            }
        }

        return neighbours;
    }

    Vector2[] RetracePath(Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }
        Vector2[] waypoints = SimplifyPath(path);
        Array.Reverse(waypoints);
        return waypoints;

    }

    Vector2[] SimplifyPath(List<Node> path)
    {
        List<Vector2> waypoints = new List<Vector2>();
        Vector2 directionOld = Vector2.zero;

        for (int i = 1; i < path.Count; i++)
        {
            Vector2 directionNew = new Vector2(path[i - 1].gridX - path[i].gridX, path[i - 1].gridY - path[i].gridY);
            if (directionNew != directionOld && !_road)
            {
                waypoints.Add(path[i].worldPosition);
            }
            else if (_road)
            {
                waypoints.Add(path[i].worldPosition);
            }
            directionOld = directionNew;
        }
        return waypoints.ToArray();
    }
}
