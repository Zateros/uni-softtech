using System;
using System.Collections;
using UnityEngine;
using static UnityEditor.PlayerSettings;
using static UnityEngine.GraphicsBuffer;

public class Turist : MonoBehaviour, IEntity
{
    private readonly float _visionRange = 2f; //TODO: finallize
    private readonly float _size = 1f; //TODO: finallize
    private readonly float _speed = 1f; //TODO: finallize
    private int _satisfaction;
    private bool inJeep = false;
    private Vehicle vehicle = null;
    protected Vector2 dir;
    protected Vector2 _position;
    private Vector2[] _path;
    private int targetIndex = 0;

    public bool IsVisible { get => true; set => throw new Exception(); }
    public int Satisfaction { get => _satisfaction; }

    private void Awake()
    {
        _position = gameObject.transform.position;

        switch (GameManager.Instance.Difficulty)
        {
            case Difficulty.EASY:
                _satisfaction = 70;
                break;
            case Difficulty.MEDIUM:
                _satisfaction = 60;
                break;
            case Difficulty.HARD:
                _satisfaction = 50;
                break;
            default:
                break;
        }
        StartCoroutine(UpdatePath());
    }

    IEnumerator UpdatePath()
    {
        if(vehicle != null)
        {
            if (Time.timeSinceLevelLoad < .3f)
            {
                yield return new WaitForSeconds(.3f);
            }
            PathManager.RequestPath(new PathRequest(transform.position, vehicle.Position, OnPathFound));
        }
        while (true && !inJeep)
        {
            if(vehicle == null || vehicle.IsFull)
            {
                vehicle = PickNearestVehicle();
            }
            else
            {
                yield return new WaitForSeconds(0.1f);
                if (targetIndex >= _path.Length)
                {
                    targetIndex = 0;
                    PathManager.RequestPath(new PathRequest(_position, vehicle.Position, OnPathFound));
                }
            }
        }
    }

    IEnumerator FollowPath()
    {
        Vector2 currentWaypoint = _path[0];
        dir = currentWaypoint.normalized;
        while (true)
        {
            if (Vector2.Distance((Vector2)transform.position, currentWaypoint) <= GameManager.Instance.eps)
            {
                targetIndex++;
                if (targetIndex >= _path.Length)
                {
                    yield break;
                }
                currentWaypoint = _path[targetIndex];
                dir = currentWaypoint.normalized;
            }
            var prevpos = _position;

            transform.position = Vector3.MoveTowards(transform.position, currentWaypoint, _speed * Time.deltaTime);
            _position = transform.position;
            if (prevpos == _position)
            {
                OnPathFound(new Vector2[0], false);
                yield break;
            }
            yield return null;
        }
    }

    public void OnPathFound(Vector2[] waypoints, bool pathSuccessful)
    {
        if (pathSuccessful)
        {
            targetIndex = 0;
            _path = waypoints;
            StopCoroutine(FollowPath());
            StartCoroutine(FollowPath());
        }
    }

    private Vehicle PickNearestVehicle()
    {
        Vehicle closest = null;
        float distance = float.PositiveInfinity;
        foreach (Vehicle vehicle in GameManager.Instance.Vehicles)
        {
            if (Vector2.Distance(_position, vehicle.Position) < distance && !vehicle.IsFull)
            {
                closest = vehicle;
                distance = Vector2.Distance(_position, vehicle.Position);
            }
        }
        return closest;
    }
}
