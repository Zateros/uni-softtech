using System;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class Vehicle : MonoBehaviour, IEntity, IPurchasable
{
    private readonly float _visionRange = 10f; //TODO: finallize
    private Vector2 _position;
    private readonly float _size = 1f; //TODO: finallize
    private readonly float _speed = 5f; //TODO: finallize
    private readonly int _price;
    private readonly int _salePrice;
    private List<Tourist> _passengers;
    private List<Vector2> _route;
    private int _routepos = 0;
    private GameObject _jeep;
    private GameManager _game;

    public bool IsVisible { get => true; set => throw new Exception(); }
    public int Cost { get => _price; }
    public bool IsFull { get => _passengers.Count == 4; }

    public Vehicle(GameObject jeep, ref GameManager game)
    {
        _passengers = new List<Tourist>();
        _position = jeep.transform.position;

        //TODO: change prices based on difficulty
        _price = 10;
        _salePrice = 2;
        _jeep = jeep;
        _game = game;
        GeneratePath();
    }

    public void Update()
    {
        Move();
    }

    public void Move()
    {
        _jeep.transform.Translate(_speed * Time.deltaTime * (_route[_routepos] - _position).normalized);
        _position += _speed * Time.deltaTime * (_route[_routepos] - _position).normalized;
        if (_route[_routepos] == _position) _routepos++;
    }

    public Vector2 GeneratePath()
    {
        Random random = new Random();
        _route = _game.Routes[random.Next(0, _game.Routes.Count)];
        return new Vector2();
    }

    public void Buy(IEntity e)
    {
        throw new NotImplementedException();
    }

    public void Place(IEntity e)
    {
        throw new NotImplementedException();
    }

    public void Sell(IEntity e)
    {
        throw new NotImplementedException();
    }
}
