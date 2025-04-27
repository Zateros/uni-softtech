using System.Collections.Generic;
using UnityEngine;

public class Hyena : Carnivore
{
    public new void Awake()
    {
        base.Awake();
        switch (GameManager.Instance.Difficulty)
        {
            case Difficulty.EASY:
                _price = 300;
                _salePrice = 300;
                break;
            case Difficulty.MEDIUM:
                _price = 400;
                _salePrice = 350;
                break;
            case Difficulty.HARD:
                _price = 500;
                _salePrice = 400;
                break;
            default:
                break;
        }
    }

    public override List<Animal> GetNeighbours(float range)
    {
        throw new System.NotImplementedException();
    }
}
