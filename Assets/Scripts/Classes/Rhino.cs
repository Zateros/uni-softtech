using UnityEngine;
using System;
using System.Collections.Generic;

public class Rhino : Herbivore
{
    public new void Awake()
    {
        base.Awake();

        switch (GameManager.Instance.Difficulty)
        {
            case Difficulty.EASY:
                _price = 1500;
                _salePrice = 1500;
                break;
            case Difficulty.MEDIUM:
                _price = 2000;
                _salePrice = 1750;
                break;
            case Difficulty.HARD:
                _price = 2500;
                _salePrice = 2000;
                break;
            default:
                break;
        }
    }

    public override List<Animal> GetNeighbours()
    {
        throw new NotImplementedException();
    }
}
