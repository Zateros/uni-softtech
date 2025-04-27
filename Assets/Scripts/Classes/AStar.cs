using System.Collections.Generic;
using System;
using UnityEngine;
using Mono.Cecil.Cil;

public class AStar : MonoBehaviour
{

    public void FindPath(PathRequest request, Action<PathResult> callback)
    {

        Vector2[] waypoints = new Vector2[0];
        bool pathSuccess = false;
        Vector3Int start = GameManager.Instance.GameTable.WorldToCell(request.pathStart);
        Vector3Int end = GameManager.Instance.GameTable.WorldToCell(request.pathEnd);
        Node startNode = GameManager.Instance.WMap[start.x, start.y];
        Node targetNode = GameManager.Instance.WMap[end.x, end.y];
        startNode.parent = startNode;

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

                if (checkX >= 0 && checkX < GameManager.Instance.GameTable.Size.x && checkY >= 0 && checkY < GameManager.Instance.GameTable.Size.y)
                {
                    neighbours.Add(GameManager.Instance.WMap[checkX, checkY]);
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
            if (directionNew != directionOld)
            {
                waypoints.Add(path[i].worldPosition);
            }
            directionOld = directionNew;
        }
        return waypoints.ToArray();
    }
}
