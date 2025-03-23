using UnityEngine;
using System;
using System.Collections.Generic;

public class Plant : MonoBehaviour, IEntity, IPurchasable
{
    protected int _visionRange;
    protected Vector2 _position;
    protected int _size;
    protected int _price;
    protected int _salePrice;
    protected int _growtime;

    public bool IsVisible { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public bool IsGrown { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public int Price { get => _price; }
    public int SalePrice { get => _salePrice; }

    public void Move()
    {
        throw new NotImplementedException();
    }

    public void GeneratePath()
    {
        throw new NotImplementedException();
    }
}
