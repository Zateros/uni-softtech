using UnityEngine;
using System;
using System.Collections.Generic;

public class Vehicle : MonoBehaviour, IEntity, IPurchasable
{
    private int _visionRange;
    private Vector2 _position;
    private int _size;
    private int _price;
    private int _salePrice;
    private List<Tourist> _passengers;

    public bool IsVisible { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public int Cost { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public bool IsFull { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public Vehicle() { }


    public void Move()
    {
        throw new NotImplementedException();
    }

    public void GeneratePath()
    {
        throw new NotImplementedException();
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
