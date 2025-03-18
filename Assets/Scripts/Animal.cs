using UnityEngine;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;

public abstract class Animal : MonoBehaviour, IEntity, IPurchasable
{
    private int _visionRange;
    private Vector2 _position;
    private int _size;
    private int _price;
    private int _salePrice;
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



    public Animal() { }


    public void Move()
    {
        throw new NotImplementedException();
    }

    public abstract void GeneratePath();

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
