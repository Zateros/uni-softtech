using UnityEngine;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using System.IO;

public abstract class Animal : MonoBehaviour, IEntity, IPurchasable
{
    protected float _visionRange;
    protected float _speed = 2f;
    protected Vector2 _position;
    private Vector2 _path;
    protected int _size;
    protected int _price;
    protected int _salePrice;
    protected int _age;
    protected int _hunger;
    protected int _thirst;
    protected bool _hasChip;
    protected int _sleepTime;
    private int framecnt = 1200;
    public Sprite _blipIcon;

    public bool IsVisible { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public Sprite BlipIcon { get => _blipIcon; set { _blipIcon = value; } }
    public bool IsAsleep { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public bool IsAdult { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public bool IsThirsty { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public bool IsHungry { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public bool IsCaptured { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public bool HasChip { get => _hasChip; set => _hasChip = value; }
    public int Price { get => _price; }
    public int SalePrice { get => _salePrice; }

    public void Awake()
    {
        _hasChip = false;
        _position = gameObject.transform.position;
        _path = GeneratePath();
    }

    public void Update()
    {
        Move();
    }

    public void Move()
    {
        if (Time.frameCount % framecnt != 0)
        {
            transform.Translate(_speed * Time.deltaTime * _path);
            _position += _speed * Time.deltaTime * _path.normalized;
        }
        else
        {
            _path = GeneratePath();
            System.Random rand = new System.Random();
            framecnt = rand.Next(900, 1100);
        }
    }

    public abstract Vector2 GeneratePath();

    public abstract void Eat(IEntity e);

    public void Drink()
    {
        throw new NotImplementedException();
    }

    public void Mate()
    {
        throw new NotImplementedException();
    }
}
