using UnityEngine;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using NUnit.Framework;

public class Tourist : MonoBehaviour, IEntity
{
    private readonly float _visionRange = 6f; //TODO: finallize
    private Vector2 _position;
    private readonly float _size = 1f; //TODO: finallize
    private readonly float _speed = 1f; //TODO: finallize
    private GameObject _tourist;
    private int _satisfaction;
    private GameManager _game;
    private bool inJeep = false;
    private Vehicle vehicle = null;
    Vector2 path;

    public bool IsVisible { get => true; set => throw new Exception(); }
    public int Satisfaction { get => _satisfaction; private set => _satisfaction = value; }

    public Tourist(GameObject tourist, ref GameManager game)
    {
        _position = tourist.transform.position;
        _tourist = tourist;
        _game = game;
        //TODO: Change starting satisfaction based on difficulty
        Satisfaction = 50;
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
            if(vehicle != null) path = vehicle.Position;
        }
    }

    private Vehicle PickNearestVehicle()
    {
        Vehicle closest = null;
        float distance = float.PositiveInfinity;
        foreach (Vehicle vehicle in _game.Vehicles)
        {
            if(!vehicle.IsFull && (vehicle.Position - _position).magnitude < distance)
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
