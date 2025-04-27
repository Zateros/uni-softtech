using System.Collections.Generic;
using UnityEngine;

public class Lion : Carnivore
{
    public new void Awake()
    {
        base.Awake();

        switch (GameManager.Instance.Difficulty)
        {
            case Difficulty.EASY:
                _price = 10;
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

    public override List<Animal> GetNeighbours()
    {
        throw new System.NotImplementedException();
    }
}
