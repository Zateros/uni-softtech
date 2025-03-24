using UnityEngine;
using System;

public class Tourist : MonoBehaviour, IEntity
{
    private readonly float _visionRange = 6f; //TODO: finallize
    private Vector2 _position;
    private readonly float _size = 1f; //TODO: finallize
    private readonly float _speed = 1f; //TODO: finallize
    private GameObject _tourist;
    private int _satisfaction;
    private bool inJeep = false;
    private Vehicle vehicle = null;
    private Vector2 path;

    public bool IsVisible { get => true; set => throw new Exception(); }
    public int Satisfaction { get => _satisfaction; }

    private void Awake()
    {
        _position = gameObject.transform.position;
        //TODO: Change starting satisfaction based on difficulty
        _satisfaction = 50;
    }

    public void Update()
    {
        if (!inJeep) Move();
    }

    public void Move()
    {
        if (vehicle != null && !vehicle.IsFull)
        {
            _tourist.transform.Translate(_speed * Time.deltaTime * (path - _position).normalized);
            _position += _speed * Time.deltaTime * (path - _position).normalized;
            if (_position == path)
            {
                vehicle.Enter(this);
                inJeep = true;
            }
        }
        else
        {
            vehicle = PickNearestVehicle();
            if (vehicle != null) path = vehicle.Position;
        }
    }

    private Vehicle PickNearestVehicle()
    {
        Vehicle closest = null;
        float distance = float.PositiveInfinity;
        foreach (GameObject obj in GameManager.Instance.Vehicles)
        {
            Vehicle vehicle = obj.GetComponent<Vehicle>();
            if (!vehicle.IsFull && (vehicle.Position - _position).magnitude < distance)
            {
                closest = vehicle;
                distance = (vehicle.Position - _position).magnitude;
            }
        }
        return closest;
    }

    public Vector2 GeneratePath()
    {
        throw new NotImplementedException();
    }
}
