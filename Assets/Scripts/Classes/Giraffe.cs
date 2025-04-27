using System.Collections.Generic;
using UnityEngine;

public class Giraffe : Herbivore
{
    public new void Awake()
    {
        base.Awake();
        switch (GameManager.Instance.Difficulty)
        {
            case Difficulty.EASY:
                _price = 2500;
                _salePrice = 2500;
                break;
            case Difficulty.MEDIUM:
                _price = 4000;
                _salePrice = 3750;
                break;
            case Difficulty.HARD:
                _price = 5000;
                _salePrice = 4000;
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
