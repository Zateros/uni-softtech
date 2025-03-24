using UnityEngine;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;

public abstract class Animal : MonoBehaviour, IEntity, IPurchasable
{    
    protected float _visionRange;
    protected Vector2 _position;
    protected int _size;
    protected int _price;
    protected int _salePrice;
    protected int _age;
    protected int _hunger;
    protected int _thirst;
    protected bool _hasChip;
    protected int _sleepTime;

    public bool IsVisible { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public bool IsAsleep { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public bool IsAdult { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public bool IsThirsty { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public bool IsHungry { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public bool IsCaptured { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public int Price { get => _price; }
    public int SalePrice { get => _salePrice; }

    public void Move()
    {
        throw new NotImplementedException();
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
