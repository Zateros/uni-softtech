using UnityEngine;
using System;
using System.Collections.Generic;

public class Plant : MonoBehaviour, IEntity, IPurchasable
{
    private int _visionRange;
    private Vector2 _position;
    private int _size;
    private int _price;
    private int _salePrice;
    private int _growtime;

    public bool IsVisible { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public bool IsGrown { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public Plant() { }


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
