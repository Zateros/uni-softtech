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
    public bool IsFull { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public int Price { get => _price; }
    public int SalePrice {  get => _salePrice; }

    public void Awake()
    {

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
    }


    public void Move()
    {
        throw new NotImplementedException();
    }

    public void GeneratePath()
    {
        throw new NotImplementedException();
    }
}
