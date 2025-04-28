using UnityEngine;
using System;
using System.Collections.Generic;

public class Vehicle : MonoBehaviour, IEntity, IPurchasable
{
    private float _visionRange = 2f;
    private Vector2 _position;
    private readonly float _size = 1f; //TODO: finallize
    private readonly float _speed = 5f; //TODO: finallize
    private int _price;
    private int _salePrice;
    private bool atEnd = false;
    private List<Turist> _passengers;
    private List<Vector2> _route;
    private int _routepos = 0;

    public Vector2 Position { get => _position; }
    public bool IsVisible { get => true; set => throw new Exception(); }
    public bool IsFull { get => _passengers.Count == 4; }

    public int Price { get => _price; }
    public int SalePrice {  get => _salePrice; }
    private bool placed = false;
    public bool Placed
    {
        get => placed; set
        {
            _position = transform.position;
            placed = true;
        }
    }

    public void Awake()
    {
        _passengers = new List<Turist>();
        switch (GameManager.Instance.Difficulty)
        {
            case Difficulty.EASY:
                _price = 60;
                _salePrice = 10;
                break;
            case Difficulty.MEDIUM:
                _price = 20;
                _salePrice = 20;
                break;
            case Difficulty.HARD:
                _price = 30;
                _salePrice = 30;
                break;
            default:
                break;
        }
        //GeneratePath();
    }


    public void Update()
    {
        //Move();
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
        throw new NotImplementedException();
    }

    public bool Enter(Turist turist)
    {
        if (IsFull) return false;
        Debug.Log("entered");
        _passengers.Add(turist);
        return true;
    }
}
