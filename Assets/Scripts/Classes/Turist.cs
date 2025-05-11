using System;
using System.Collections;
using UnityEngine;

public class Turist : MonoBehaviour, IEntity
{
    private readonly float _visionRange = 2f; //TODO: finallize
    private readonly float _size = 1f; //TODO: finallize
    private readonly float _speed = 1f; //TODO: finallize
    private int _satisfaction;
    private bool inJeep = false;
    private bool _enRoute = false;
    private Vehicle vehicle = null;
    protected Vector2 dir;
    protected Vector2 _position;
    private Vector2[] _path;
    private int targetIndex = 0;

    public bool IsVisible { get => true; set => throw new Exception(); }
    public int Satisfaction { get => _satisfaction; }

    private void Awake()
    {
        _position = transform.position;

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
    }

    void Update()
    {
        if (!inJeep && !_enRoute)
        {
            vehicle = PickNearestVehicle();
            if (vehicle != null)
            {
                PathManager.RequestPath(new PathRequest(_position, vehicle.Position, OnPathFound));
                _enRoute = true;
            }
        }
    }

    IEnumerator FollowPath()
    {
        Vector2 currentWaypoint = _path[0];
        while (true)
        {
            if(vehicle.IsFull)
            {
                _enRoute = false;
                yield break;
            }
            if (_position == currentWaypoint)
            {
                targetIndex++;
                if (targetIndex >= _path.Length)
                {
                    bool succes = vehicle.Enter(this);
                    if (succes)
                    {
                        _position = vehicle.Position;
                        transform.position = new Vector3(_position.x, _position.y,-10);
                    }
                    inJeep = succes;
                    _enRoute = false;
                    yield break;
                }
                currentWaypoint = _path[targetIndex];
            }

            transform.position = Vector3.MoveTowards(transform.position, currentWaypoint, _speed * Time.deltaTime);
            _position = transform.position;
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
        float distance = -1;
        foreach (Vehicle vehicle in GameManager.Instance.Vehicles)
        {
            if(distance == -1 && Vector2.Distance(_position, vehicle.Position) <= _visionRange && !vehicle.IsFull)
            {
                closest = vehicle;
                distance = Vector2.Distance(_position, vehicle.Position);
            }
            else if (Vector2.Distance(_position, vehicle.Position) < distance && !vehicle.IsFull)
            {
                closest = vehicle;
                distance = Vector2.Distance(_position, vehicle.Position);
            }
        }
        return closest;
    }
}
