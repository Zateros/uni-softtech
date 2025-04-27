using UnityEngine;
using System;
using System.Collections.Generic;
using Random = System.Random;

public class Vehicle : MonoBehaviour, IEntity, IPurchasable
{
    private int _visionRange;
    private Vector2 _position;
    private readonly float _size = 1f; //TODO: finallize
    private readonly float _speed = 5f; //TODO: finallize
    private int _price;
    private int _salePrice;
    private bool atEnd = false;
    private List<Turist> _passengers;
    private List<Vector2> _route;
    private int _routepos = 0;

    public Vector2 Position { get; }
    public bool IsVisible { get => true; set => throw new Exception(); }
    public bool IsFull { get => _passengers.Count == 4; }

    public int Price { get => _price; }
    public int SalePrice {  get => _salePrice; }
    private bool placed = false;
    public bool Placed
    {
        get => placed; set
        {
            _position = gameObject.transform.position;
            placed = true;
        }
    }

    public void Awake()
    {
        _passengers = new List<Turist>();
        switch (GameManager.Instance.Difficulty)
        {
            case Difficulty.EASY:
                _price = 20000;
                _salePrice = 20000;
                break;
            case Difficulty.MEDIUM:
                _price = 25000;
                _salePrice = 22500;
                break;
            case Difficulty.HARD:
                _price = 30000;
                _salePrice = 25000;
                break;
            default:
                break;
        }
        _position = gameObject.transform.position;
        GeneratePath();
    }


    public void Update()
    {
        Move();
    }

    public void Move()
    {
        if(_route != null)
        {
            if (IsFull && !atEnd)
            {
                gameObject.transform.Translate(_speed * Time.deltaTime * (_route[_routepos] - _position).normalized);
                _position += _speed * Time.deltaTime * (_route[_routepos] - _position).normalized;
                if (Vector2.Distance(_route[_routepos], _position) <= GameManager.Instance.eps) _routepos++;
                if (_routepos == _route.Count)
                {
                    atEnd = true;
                    _passengers.Clear();
                    _routepos--;
                }
            }
            else if (atEnd)
            {
                gameObject.transform.Translate(_speed * Time.deltaTime * (_route[_routepos] - _position).normalized);
                _position += _speed * Time.deltaTime * (_route[_routepos] - _position).normalized;
                if (Vector2.Distance(_route[_routepos], _position) <= GameManager.Instance.eps) _routepos--;
                if (_routepos == -1)
                {
                    atEnd = false;
                    GeneratePath();
                    _routepos = 0;
                }
            }
        }
        else
        {
            GeneratePath();
        }
    }

    public Vector2 GeneratePath()
    {
        if (GameManager.Instance.Routes.Count > 0)
        {
            Random random = new Random();
            _route = GameManager.Instance.Routes[random.Next(0, GameManager.Instance.Routes.Count)];
        }
        return new Vector2();
    }

    public void Enter(Turist turist)
    {
        if (IsFull) return;
        _passengers.Add(turist);
    }
}
