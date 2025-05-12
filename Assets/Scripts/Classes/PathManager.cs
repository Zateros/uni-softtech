using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Threading;

public class PathManager : MonoBehaviour
{

    Queue<PathResult> results = new Queue<PathResult>();

    static PathManager instance;
    AStar pathfinding;

    void Awake()
    {
        instance = this;
        pathfinding = GetComponent<AStar>();
    }

    void Update()
    {
        if (results.Count > 0)
        {
            int itemsInQueue = results.Count;
            lock (results)
            {
                for (int i = 0; i < itemsInQueue; i++)
                {
                    PathResult result = results.Dequeue();
                    result.callback(result.path, result.success);
                }
            }
        }
    }

    /// <summary>
    /// Creates a path request
    /// </summary>
    /// <param name="request"></param>
    /// <param name="road">only path on roads</param>
    public static void RequestPath(PathRequest request, bool road)
    {
        ThreadStart threadStart = delegate {
            instance.pathfinding.FindPath(request, instance.FinishedProcessingPath, road);
        };
        threadStart.Invoke();
    }

    public void FinishedProcessingPath(PathResult result)
    {
        lock (results)
        {
            results.Enqueue(result);
        }
    }



}

public struct PathResult
{
    public Vector2[] path;
    public bool success;
    public Action<Vector2[], bool> callback;

    public PathResult(Vector2[] path, bool success, Action<Vector2[], bool> callback)
    {
        this.path = path;
        this.success = success;
        this.callback = callback;
    }

}

public struct PathRequest
{
    public Vector3 pathStart;
    public Vector3 pathEnd;
    public Action<Vector2[], bool> callback;

    public PathRequest(Vector3 _start, Vector3 _end, Action<Vector2[], bool> _callback)
    {
        pathStart = _start;
        pathEnd = _end;
        callback = _callback;
    }

}
