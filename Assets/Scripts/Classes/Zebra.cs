using System.Collections.Generic;
using UnityEngine;

public class Zebra : Herbivore
{
    public new void Awake()
    {
        base.Awake();

        switch (GameManager.Instance.Difficulty)
        {
            case Difficulty.EASY:
                _price = 500;
                _salePrice = 500;
                break;
            case Difficulty.MEDIUM:
                _price = 650;
                _salePrice = 600;
                break;
            case Difficulty.HARD:
                _price = 800;
                _salePrice = 700;
                break;
            default:
                break;
        }
    }

    public override List<Animal> GetNeighbours()
    {
        throw new System.NotImplementedException();
    }
}
